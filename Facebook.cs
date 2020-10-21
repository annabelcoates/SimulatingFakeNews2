using System;
using System.Collections.Generic;

namespace ModelAttemptWPF
{
    public class Facebook : OSN
    {
        public Facebook(string name, int feedTimeFrame):base(name)
        {
            this.feedTimeFrame = feedTimeFrame;
        }

        public new void CreateRandomMutualFollows(Account account, int nConnections)
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
                        Follow(accountList[randomID], accountList[account.ID]);
                        connectionsNotFound = false;
                    }
                }
            }
        }

        public new void CreateMutualFollowsFromGraph(string filePath)
        {
            List<string[]> connections = LoadCsvFile(filePath);
            foreach (string[] connection in connections)
            {
                // string[0] is the key and isn't necesary
                int followerID = Convert.ToInt16(connection[1]);
                int followeeID = Convert.ToInt16(connection[2]);
                this.Follow(accountList[followeeID], accountList[followerID]);
                this.Follow(accountList[followerID], accountList[followeeID]);
            }
        }
        public new void CreateFollowsBasedOnPersonality(int defaultFollows)
        //Change this to take a default number of follows
        {
            foreach (Account account in this.accountList)
            {
                int nConnections = Convert.ToInt16(account.person.largeNetwork * defaultFollows);
                this.CreateRandomMutualFollows(account, nConnections);
            }
        }
    }
}