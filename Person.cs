using ModelAttemptWPF;
using System;

public class Person
{
    public int ID;

    // The 'Big 5' Personality traits
    public double o; // Openess
    public double c; // Conscientiousness
    public double e; // Extraversion
    public double a; // Agreeableness
    public double n; // Neuroticism

   

    public double freqUse; // A measure of how likely a user is to check social media in a 15 min timeslot, 0-1
    public double sessionLength; // A measure of how many posts a person sees in 1 view of their feed, 0-1, Maybe should be int?
    public double largeNetwork; // A measure of how likely someone is to have a large network group, can be greater than one

    public double emotionalState; // Represents how emotional someone feels, 0-1
    public double politicalLeaning; // 0 represents 'the left', 1 represents 'the right'
    public double onlineLiteracy; // how likely someone is to believe fake news
    public bool informationLiterate;
    public double sharingFreq;

    public string name;
    public int nFakeShares;
    public int nTrueShares;

    public Random random = new Random();
    public Person(int ID,string name,double o, double c, double e, double a, double n, double politicalLeaning, double onlineLiteracy)
	{
        this.ID = ID;
        this.name = name;
        // Make these normalised?
        this.o = o;
        this.c = c;
        this.e = e;
        this.a = a;
        this.n = n;

        this.politicalLeaning = politicalLeaning;
        //this.onlineLiteracy = onlineLiteracy;

        this.onlineLiteracy = onlineLiteracy;
        this.emotionalState = 0.5; // emotional state starts average
        this.DetermineComplexBehaviours(); // set the behavioural parameters based on the personality traits
	}

    

    public void DetermineComplexBehaviours()
    {
        // From caci et al 2014 path analysis
        this.freqUse = -0.18 * this.c + 0.12 * this.e - 0.21 * this.a + 0.14 * this.n;
        this.freqUse += 0.5; // to ensure it is above 0
        this.freqUse *= this.freqUse; // to reintroduce a contrast in the spread of frequencies
        if (this.freqUse < 0)
        {
            Console.WriteLine(this.name+" freqUse below 0: " + freqUse);
        }
        

        this.sessionLength = -0.28 * this.c + 0.24 * e + 0.14 * this.n+0.5;
        this.sessionLength *= this.sessionLength;
        if (this.sessionLength < 0)
        {
            Console.WriteLine(this.name+" sessionLength  below 0: " + freqUse);
        }
        this.largeNetwork = Math.Max(0,(-0.28 * this.c + 0.24 * this.o + 0.47 * this.e - 0.28 * this.a)); // can be larger than 1
        // research on likelihood of sharing needed
        this.sharingFreq = this.e * this.e;
    }


    public double AssesNews(News news)
    {
        // Currently the news is assessed according to 3 factors equally, politics, emotional level and believability

        // political factor is higher if the political leanings are closer
        double politicalFactor = 1 - Math.Abs(news.politicalLeaning - this.politicalLeaning);

        // how much the news appeals emotionally increases with the person's emotional level and how emotional the news is
        double emotionalFactor = this.n * news.emotionalLevel;

        double believabilityFactor = (news.believability/onlineLiteracy);
        //believabilityFactor = 1 - onlineLiteracy;
        // The perceived believability is dependent on the believability of the article and the person's online literacy
        

        // According to Pennycook & Rand (2018) failing to identify news is fake is the biggest affector of how likely a person is to believe and therefore share it (partisanship/ political factor is more minor)

        double shareProb = this.sharingFreq *((politicalFactor +emotionalFactor)*believabilityFactor);
        //Console.WriteLine(this.name+" probability of sharing "+news.name+": "  shareProb);
        // return the likelihood that someone will share the news
        return shareProb;

    }
}
