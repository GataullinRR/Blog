using System;
using System.Drawing;

namespace StatisticServiceExports.Kafka
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

        public RegisteredInfo Registered { get; set; }
        public StateChangedInfo StateChanged { get; set; }

        public UserNotification(RegisteredInfo registered)
        {
            Registered = registered ?? throw new ArgumentNullException(nameof(registered));
        }
        public UserNotification(StateChangedInfo stateChanged)
        {
            StateChanged = stateChanged ?? throw new ArgumentNullException(nameof(stateChanged));
        }

        public void Deconstruct(out RegisteredInfo registered, out StateChangedInfo stateChanged)
        {
            registered = Registered;
            stateChanged = StateChanged;
        }
    }
}
