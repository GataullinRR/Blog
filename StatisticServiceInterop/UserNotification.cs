using System;

namespace StatisticServiceExports
{
    [Serializable]
    public class UserNotification
    {
        [Serializable]
        public class RegisteredInfo
        {
            public int ProfileState { get; set; }

            public RegisteredInfo(int profileState)
            {
                ProfileState = profileState;
            }
        }

        [Serializable]
        public class StateChangedInfo
        {
            public int OldProfileState { get; set; }
            public int NewProfileState { get; set; }

            public StateChangedInfo(int oldProfileState, int newProfileState)
            {
                OldProfileState = oldProfileState;
                NewProfileState = newProfileState;
            }
        }

        [Serializable]
        public class ActionPerformedInfo
        {
            public int ActionType { get; }

            public ActionPerformedInfo(int actionType)
            {
                ActionType = actionType;
            }
        }

        public RegisteredInfo Registered { get; set; }
        public StateChangedInfo StateChanged { get; set; }
        public ActionPerformedInfo ActionPerformed { get; set; }

        public UserNotification(RegisteredInfo registered)
        {
            Registered = registered ?? throw new ArgumentNullException(nameof(registered));
        }
        public UserNotification(StateChangedInfo stateChanged)
        {
            StateChanged = stateChanged ?? throw new ArgumentNullException(nameof(stateChanged));
        }
        public UserNotification(ActionPerformedInfo actionPerformed)
        {
            ActionPerformed = actionPerformed ?? throw new ArgumentNullException(nameof(actionPerformed));
        } 
    }
}
