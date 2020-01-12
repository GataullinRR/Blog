using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBModels;

namespace Blog.Models
{
    public class AdminPanelModeratorsTabModel
    {
        public AdminPanelModeratorsTabModel(SummaryModel summary)
        {
            Summary = summary ?? throw new ArgumentNullException(nameof(summary));
        }

        public class SummaryModel
        {
            /// <summary>
            /// Describes <see cref="ModeratorsGroup"/> or moderator inside the group
            /// </summary>
            public class GroupInfo
            {
                public GroupInfo(string groupName, int[] resolvedEntities, double[] summedResolveTime, GroupInfo[] subGroupsInfo)
                {
                    GroupName = groupName ?? throw new ArgumentNullException(nameof(groupName));
                    ResolvedEntities = resolvedEntities ?? throw new ArgumentNullException(nameof(resolvedEntities));
                    SummedResolveTime = summedResolveTime ?? throw new ArgumentNullException(nameof(summedResolveTime));
                    SubGroups = subGroupsInfo;
                }

                public string GroupName { get; }
                public int[] ResolvedEntities { get; }
                /// <summary>
                /// Total duration of time to resolve for each resolved entity.
                /// <see cref="double.NaN"/> if not provided
                /// </summary>
                public double[] SummedResolveTime { get; }
                /// <summary>
                /// Used for moderators
                /// </summary>
                public GroupInfo[] SubGroups { get; }
            }

            public DateTime[] XAxis { get; }
            public GroupInfo[] Groups { get; }

            public SummaryModel(DateTime[] xAxis, GroupInfo[] groups)
            {
                XAxis = xAxis ?? throw new ArgumentNullException(nameof(xAxis));
                Groups = groups ?? throw new ArgumentNullException(nameof(groups));
            }
        }

        public SummaryModel Summary { get; }
    }
}
