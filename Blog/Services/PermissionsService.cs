using ASPCoreUtilities;
using Blog.Attributes;
using Blog.Models;
using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Utilities.Extensions;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class PermissionsService : ServiceBase
    {
        public const int MAX_POST_EDITS_FOR_STANDARD_USER = 3;
        public const int NUM_OF_DAYS_USER_CAN_EDIT_OWN_POST = 3;
        public const int MAX_COMMENTARY_EDITS_FOR_STANDARD_USER = 1;

        public PermissionsService(ServiceLocator services) : base(services)
        {

        }

        #region ### Posts permissions ###

        public async Task ValidateViewPostAsync(Post post)
        {
            if (!await CanViewPostAsync(post))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanViewPostAsync(Post post)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            var canNotAuthenticatedUserViewPost = post.ModerationInfo.State == ModerationState.MODERATED
                && !post.IsDeleted;
            if (currentUser == null)
            {
                return canNotAuthenticatedUserViewPost;
            }
            else
            {
                // to make explicit loading work for the case when post was loaded in non-tracking context
                post = await S.Db.FindAsync<Post>(post.Id);
                await S.Db.Entry(post).Reference(p => p.Author).LoadAsync();
                return currentUser.Id == post.Author.Id
                    || (currentUser.Role == Role.USER && canNotAuthenticatedUserViewPost)
                    || currentUser.Role > Role.USER;
            }
        }

        public async Task ValidateCreatePostAsync()
        {
            if (!await CanCreatePostAsync())
            {
                throw buildException();
            }
        }
        public async Task<bool> CanCreatePostAsync()
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return user.Role >= Role.USER;
            }
        }

        public async Task ValidateEditPostAsync(Post post)
        {
            if (!await CanEditPostAsync(post))
            {
                throw buildException($"The post \"{post.Title}\" can not be edited by current user");
            }
        }
        public async Task<bool> CanEditPostAsync(Post post)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE || post.IsDeleted)
            {
                return false;
            }
            else
            {
                var notYetPublished = post.ModerationInfo.State != ModerationState.MODERATED;
                var canEditAsAuthor = user.Role == Role.USER && user.Id == post.Author.Id
                    && (notYetPublished
                    || (DateTime.UtcNow - post.CreationTime) < TimeSpan.FromDays(NUM_OF_DAYS_USER_CAN_EDIT_OWN_POST)
                        && post.Edits.Where(e => e.Author.Id == post.Author.Id && e.MadeWhilePublished).Count() < MAX_POST_EDITS_FOR_STANDARD_USER);
                return canEditAsAuthor
                    || user.Role == Role.MODERATOR
                    || (user.Role == Role.ADMINISTRATOR
                        && user.Id == post.Author.Id);
            }
        }

        public async Task<IQueryable<Post>> CanEditPostTitleIncludeAsync(IQueryable<Post> postQuery)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                return postQuery;
            }
            else
            {
                return postQuery
                        .Include(p => p.ModerationInfo)
                        .Include(p => p.Author);
            }
        }
        public async Task<bool> CanEditPostTitleAsync(Post post)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await CanEditPostAsync(post)
                    && (user.Role >= Role.MODERATOR
                        || post.ModerationInfo.State.IsOneOf(ModerationState.MODERATION_NOT_PASSED, ModerationState.UNDER_MODERATION));
            }
        }

        public async Task ValidateDeletePostAsync(Post post)
        {
            if (!await CanDeletePostAsync(post))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanDeletePostAsync(Post post)
        {
            return await canDeleteAsync(post);
        }

        public async Task ValidateRestorePostAsync(Post post)
        {
            if (!await CanRestorePostAsync(post))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanRestorePostAsync(Post post)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return post.IsDeleted
                    && user.Role >= Role.MODERATOR
                    && !await CanDeletePostAsync(post);
            }
        }

        public async Task<bool> CanCreatePostsWithoutModerationAsync()
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return user.Role >= Role.MODERATOR;
            }
        }

        #endregion

        #region ### Commentaries permissions ###

        readonly Dictionary<int[], IQueryable<CommentaryPermissionsModel>> _commentaryPermissionsCache 
            = new Dictionary<int[], IQueryable<CommentaryPermissionsModel>>();

        public async Task<IQueryable<CommentaryPermissionsModel>> GetCommentaryPermissionsAsync(int[] commentaryIds)
        {
            var key = _commentaryPermissionsCache.Keys.FirstOrDefault(k => k.SequenceEqual(commentaryIds));
            if (key != null)
            {
                return _commentaryPermissionsCache[key];
            }

            var user = await getCurrentUserOrNullAsync();
            IQueryable<CommentaryPermissionsModel> result;
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                result = new CommentaryPermissionsModel()
                    .Repeat(commentaryIds.Length)
                    .AsAsyncQuerable();
            }
            else
            {
                var isPowered = user.Role >= Role.MODERATOR;
                result = S.Db.Commentaries.AsNoTracking()
                        .Where(c => commentaryIds.Contains(c.Id))
                        .Select(c => new CommentaryPermissionsModel
                        {
                            CanDelete = !c.IsDeleted && isPowered,
                            CanRestore = c.IsDeleted && isPowered,
                            CanEdit = !c.IsDeleted && (isPowered ||
                         (!c.IsHidden &&
                            c.Author.Id == user.Id &&
                            (c.CreationTime - DateTime.UtcNow).TotalDays < 1 &&
                            c.Edits.Count(e => e.Author.Id == user.Id) < 1)),
                            CanReport = !c.IsDeleted &&
                             c.Author.Id != user.Id &&
                             c.Reports.Any(r => r.Reporter.Id == user.Id),
                            CanReportViolation = !c.IsDeleted &&
                             isPowered &&
                             c.Author.Id != user.Id &&
                             c.Violations.Any(r => r.Reporter.Id == user.Id)
                        });
            }

            _commentaryPermissionsCache[commentaryIds] = result;
            return result;
        }
        public async Task<CommentaryPermissionsModel> GetCommentaryPermissionAsync(int commentaryId)
        {
            var permissions = await GetCommentaryPermissionsAsync(new int[] { commentaryId });
            return await permissions.SingleAsync();
        }

        public async Task ValidateAddCommentaryAsync(Post post)
        {
            if (!await CanAddCommentaryAsync(post))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanAddCommentaryAsync(Post post)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotDeletedAsync(post)
                    && post.ModerationInfo.State == ModerationState.MODERATED;
            }
        }

        public async Task ValidateEditCommentaryAsync(Commentary comment)
        {
            if (!await CanEditCommentaryAsync(comment))
            {
                throw buildException($"The commentary \"{comment.Id}\" can not be edited by current user");
            }
        }
        public async Task<bool> CanEditCommentaryAsync(Commentary comment)
        {
            return await GetCommentaryPermissionAsync(comment.Id).ThenDo(p => p.CanEdit);
        }

        public async Task ValidateDeleteCommentaryAsync(Commentary comment)
        {
            if (!await CanDeleteCommentaryAsync(comment))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanDeleteCommentaryAsync(Commentary comment)
        {
            return await GetCommentaryPermissionAsync(comment.Id).ThenDo(p => p.CanDelete);
        }

        public async Task ValidateRestoreCommentaryAsync(Commentary comment)
        {
            if (!await CanRestoreCommentaryAsync(comment))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanRestoreCommentaryAsync(Commentary comment)
        {
            return await GetCommentaryPermissionAsync(comment.Id).ThenDo(p => p.CanRestore);
        }

        #endregion

        #region ### Misc permissions ###

        readonly Dictionary<string, ProfilePermissions> _profilePermissionsCache = new Dictionary<string, ProfilePermissions>();

        public async Task<ProfilePermissions> GetProfilePermissionsAsync(string targetUserId)
        {
            if (_profilePermissionsCache.ContainsKey(targetUserId))
            {
                return _profilePermissionsCache[targetUserId];
            }

            ProfilePermissions result;
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE && targetUserId != user.Id)
            {
                result = new ProfilePermissions();
            }
            else
            {
                var targetUser = await S.Db.Users.AsNoTracking()
                    .Include(u => u.Reports)
                    .ThenInclude(v => v.Reporter)
                    .Include(u => u.Violations)
                    .Include(u => u.Status)
                    .FirstOrDefaultAsync(u => u.Id == targetUserId);
                var isSameUser = user.Id == targetUser.Id;
                var isPowered = user.Role >= Role.MODERATOR;
                result = new ProfilePermissions()
                {
                    CanBan = user.Status.State != ProfileState.BANNED &&
                        user.Role > targetUser.Role &&
                        isPowered,
                    CanUnbanUser = user.Status.State == ProfileState.BANNED &&
                        user.Role > targetUser.Role &&
                        isPowered,
                    CanSeeServiceInformation = user.Role >= Role.MODERATOR,
                    CanReport = !isSameUser &&
                        !targetUser.Reports.Any(r => r.Reporter.Id == user.Id),
                    CanReportViolation = !isSameUser &&
                        isPowered,
                    CanSeeTabs = isSameUser ||
                        isPowered,
                    CanChangePassword = isSameUser &&
                        targetUser.EmailConfirmed &&
                        targetUser.Status.State == ProfileState.ACTIVE,
                    CanSeePrivateInformation = isSameUser ||
                        isPowered,
                    CanEdit = isSameUser ||
                        targetUser.Role < user.Role,
                };
                result.CanSeeActionsTab = result.CanSeeTabs;
                result.CanSeeSettingsTab = result.CanSeeTabs;
                result.CanSeeGeneralInformation = isSameUser || result.CanSeeTabs;
                result.CanChangeEMail = result.CanChangePassword;
            }

            _profilePermissionsCache[targetUserId] = result;
            return result;
        }

        public async Task ValidateResetPasswordAsync(User targetUser)
        {
            if (!await CanResetPasswordAsync(targetUser))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanResetPasswordAsync(User targetUser)
        {
            return ((targetUser.Status.LastPasswordRestoreAttempt ?? DateTime.UtcNow.AddDays(-999)) - DateTime.UtcNow).TotalMinutes.Abs() > 30
                        && targetUser.EmailConfirmed;
        }

        public async Task ValidateChangePasswordAsync(User targetUser)
        {
            if (!await CanChangePasswordAsync(targetUser))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanChangePasswordAsync(User targetUser)
        {
            return await GetProfilePermissionsAsync(targetUser.Id).ThenDo(p => p.CanChangePassword);
        }

        public async Task ValidateChangeEmailAsync(User targetUser)
        {
            if (!await CanChangeEmailAsync(targetUser))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanChangeEmailAsync(User targetUser)
        {
            return await GetProfilePermissionsAsync(targetUser.Id).ThenDo(p => p.CanChangeEMail);
        }

        public async Task ValidateBanUserAsync(User targetUser)
        {
            if (!await CanBanUserAsync(targetUser))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanBanUserAsync(User targetUser)
        {
            return await GetProfilePermissionsAsync(targetUser.Id).ThenDo(p => p.CanBan);
        }

        public async Task ValidateUnbanUserAsync(string targetUserId)
        {
            if (!await CanUnbanUserAsync(targetUserId))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanUnbanUserAsync(string targetUserId)
        {
            return await GetProfilePermissionsAsync(targetUserId).ThenDo(p => p.CanUnbanUser);
        }

        public async Task ValidateLogoutAsync()
        {
            if (!await CanLogoutAsync())
            {
                throw buildException();
            }
        }
        public async Task<bool> CanLogoutAsync()
        {
            return S.HttpContext.User?.Identity?.IsAuthenticated ?? false;
        }

        public async Task<bool> CanSeePrivateInformationAsync(User targetUser)
        {
            return await GetProfilePermissionsAsync(targetUser.Id).ThenDo(p => p.CanSeePrivateInformation);
        }

        public async Task<bool> CanSeeServiceInformationAsync()
        {
            var user = await getCurrentUserOrNullAsync();
            return user.Role >= Role.MODERATOR;
        }

        public async Task<bool> CanEditProfileAsync(User targetUser)
        {
            return await GetProfilePermissionsAsync(targetUser.Id).ThenDo(p => p.CanEdit);
        }

        public async Task ValidateEditProfileAsync(User targetUser)
        {
            if (!await CanEditProfileAsync(targetUser))
            {
                throw buildException();
            }
        }

        public async Task<bool> CanEditProfileWithoutCheckAsync(User targetUser)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null)
            {
                return false;
            }
            else
            {
                return await CanEditProfileAsync(targetUser)
                    && currentUser.Id != targetUser.Id
                    && currentUser.Role >= Role.MODERATOR;
            }
        }

        public async Task ValidateReportAsync(IReportable reportObject)
        {
            if (!await CanReportAsync(reportObject))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanReportAsync(IReportable reportObject)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotDeletedAsync(reportObject)
                    && currentUser.Id != reportObject.Author.Id
                    && !reportObject.Reports.Any(r => r.Reporter.Id == currentUser.Id) // Already reported
                    && (reportObject.As<IModeratable>()?.ModerationInfo?.State ?? ModerationState.MODERATED) == ModerationState.MODERATED
                    && currentUser.Role == Role.USER;
            }
        }

        public async Task ValidateReportViolationAsync(IReportable reportObject)
        {
            if (!await CanReportViolationAsync(reportObject))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanReportViolationAsync(IReportable reportObject)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotDeletedAsync(reportObject)
                    && currentUser.Id != reportObject.Author.Id
                    && currentUser.Role >= Role.MODERATOR
                    && (reportObject.As<IModeratable>()?.ModerationInfo?.State ?? ModerationState.MODERATED) == ModerationState.MODERATED;
            }
        }

        public async Task ValidateMarkAsModeratedAsync(IModeratable moderatable)
        {
            if (!await CanMarkAsModeratedAsync(moderatable))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanMarkAsModeratedAsync(IModeratable entity)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotDeletedAsync(entity)
                    && (currentUser.Role == Role.MODERATOR
                        || currentUser.Role > Role.MODERATOR && currentUser.Id == entity.Author.Id)
                    &&  entity.ModerationInfo.State.IsOneOf(ModerationState.UNDER_MODERATION);
            }
        }

        public async Task ValidateAccessModeratorsPanelAsync(User target)
        {
            if (!await CanAccessModeratorsPanelAsync(target))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanAccessModeratorsPanelAsync(User target)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return (currentUser.Role >= Role.ADMINISTRATOR
                        && currentUser.Id != target.Id)
                    || (currentUser.Role == Role.MODERATOR
                        && currentUser.Id == target.Id);
            }
        }

        public async Task ValidateAccessModeratorsPanelUsersTableAsync(ModeratorsGroup target)
        {
            if (!await CanAccessModeratorsPanelUsersTableAsync(target))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanAccessModeratorsPanelUsersTableAsync(ModeratorsGroup target)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return currentUser.Role >= Role.MODERATOR;
            }
        }

        public async Task ValidateMarkAsResolvedAsync(User target)
        {
            if (!await CanMarkAsResolvedAsync(target))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanMarkAsResolvedAsync(User target)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await CanAccessModeratorsPanelAsync(target)
                    && currentUser.Id == target.Id;
            }
        }

        public async Task ValidateAccessBlogControlPanelAsync()
        {
            if (!await CanAccessAdminPanelAsync())
            {
                throw buildException();
            }
        }
        public async Task<bool> CanAccessAdminPanelAsync()
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return currentUser.Role == Role.ADMINISTRATOR;
            }
        }

        public async Task ValidateGenerateActivationLinkAsync(ActivationLinkAction activationLinkAction)
        {
            if (!await CanGenerateActivationLinkAsync(activationLinkAction))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanGenerateActivationLinkAsync(ActivationLinkAction activationLinkAction)
        {
            return await CanAccessAdminPanelAsync();
        }

        public async Task ValidateMarkAsNotPassedModerationAsync(int postId)
        {
            if (!await CanMarkAsNotPassedModerationAsync(postId))
            {
                throw buildException();
            }
        }

        public async Task<bool> CanMarkAsNotPassedModerationAsync(Post post)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotDeletedAsync(post)
                    && currentUser.Id != post.Author.Id
                    && post.ModerationInfo.State == ModerationState.UNDER_MODERATION
                    && post.Author.Role == Role.MODERATOR;
            }
        }
        public async Task<bool> CanMarkAsNotPassedModerationAsync(int postId)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                var post = await S.Db.Posts.AsNoTracking()
                    .Include(p => p.ModerationInfo)
                    .Include(p => p.Author)
                    .FirstOrDefaultAsync(p => p.Id == postId);
                return await isNotDeletedAsync(post)
                    && currentUser.Id != post.Author.Id
                    && post.ModerationInfo.State == ModerationState.UNDER_MODERATION
                    && post.Author.Role == Role.MODERATOR;
            }
        }

        #endregion

        async Task<bool> canDeleteAsync(IDeletable deletable)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotDeletedAsync(deletable)
                    && !deletable.IsDeleted
                    && user.Role >= Role.MODERATOR;
            }
        }

        async Task<bool> isNotDeletedAsync(object entity)
        {
            if (entity is IDeletable deletable)
            {
                return !deletable.IsDeleted;
            }
            else
            {
                return true;
            }
        }

        async Task<User> getCurrentUserOrNullAsync()
        {
            return S.HttpContext.User?.Identity?.IsAuthenticated ?? false
                ? await getUserAsync()
                : null;

            async Task<User> getUserAsync()
            {
                var user = await S.UserManager.GetUserAsync(S.HttpContext.User);
                return user == null
                    ? null
                    : await S.Db.Users.AsNoTracking()
                      .Include(u => u.Status)
                      .Include(u => u.Profile)
                      .FirstOrDefaultAsync(u => u.Id == user.Id);
            }
        }

        Exception buildException(string reason = null)
        {
            S.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            return new UnauthorizedAccessException(reason);
        }
    }
}