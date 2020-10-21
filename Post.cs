using System;

namespace ModelAttemptWPF
{
    public class Post
    {
        public News news;
        public Account poster;
        public int time;
        public int popularity;
        public int uniqueViews;
        public int totalViews;

        public Post(News news, int time, Account poster)
        {
            this.news = news;
            this.time = time;
            this.poster = poster;
        }
    }
}