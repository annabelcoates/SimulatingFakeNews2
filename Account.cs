using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModelAttemptWPF
{
    
    public class Account
    {
        public OSN osn;
        public int ID;
        public List<Account> following=new List<Account>(); // A list of all the accounts this account follows
        public List<Account> followers=new List<Account>();


        public List<Post> page= new List<Post>(); // A list of posts this account has shared
        public List<Post> feed= new List<Post>(); // A list of posts that the account's follows have shared
        public List<News> seen = new List<News>(); // a list of all the posts the user has seen
        private Random random= new Random();

        public Person person;

        public Account(OSN osn, Person person)
        {
            this.osn = osn;
            this.ID = osn.IDCount;
            this.person = person;
        }



        private void ShareNews(News news, int hourOfDay)
        { 
            // can only be called by CreateFakeNews(), CreateTrueNews() and ViewFeed()
                Post newPost = new Post(news, hourOfDay,this);
                this.page.Add(newPost);
        }



        public void Follow(Account followingAccount)
        {
            followingAccount.followers.Add(this);
            this.following.Add(followingAccount);
        }

        public bool HasSeen(News news)
        {
            return this.seen.Contains(news);
        }

        public bool HasShared(News news)
        {
            foreach(Post post in this.page)
            {
                if (post.news.name == news.name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}