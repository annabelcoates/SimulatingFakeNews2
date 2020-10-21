using System;
using System.Collections.Generic;

namespace ModelAttemptWPF
{
    public class Twitter : OSN
    {
        public Twitter(string name):base(name)
        {
            this.chronology = 2000;

        }
        public void CreateRandomFollows(Account account, int nConnections)
        {
            List<int> connectionIDS = new List<int>();
            bool connectionsNotFound = true;

            for (int i = 0; i < nConnections; i++)
            {
                connectionsNotFound = true;
                while (connectionsNotFound)
                {
                    int randomID = random.Next(0, IDCount);
                    if ((randomID != account.ID) & (connectionIDS.Contains(randomID) == false))
                    {
                        connectionIDS.Add(randomID); // use the list to keep track of who has already been followed
                        Follow(accountList[account.ID], accountList[randomID]);
                        connectionsNotFound = false;
                    }
                }
            }
        }
    }
}

