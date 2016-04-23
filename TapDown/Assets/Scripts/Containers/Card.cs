

public class Card  {
    public string type = "";
    public int attack = 0;
    public bool atk = false;
    public bool toDown = false;
    public bool pin = false;
    public bool downOpp = false;
    public bool oppSkip = false;
    public bool counter = false;
    public bool oppDisc = false;
    public bool draw = false;
    public int numDraw = 0;
    public bool stand = false;
    public bool escape = false;
    public bool submit = false;
    public bool special = false;
    public string name = "default";
    public int num;

    
    public Card(string name)
    {
        this.name = name;
        if(!name.ToLower().Equals("no counter"))
        {
            string[] card = name.Split('_');
            Character temp;
            num = int.Parse(card[1]);
            if (card[0].Equals("cardHero"))
            {
                temp = new Hero();
                makeCard(temp.cards[num]);
            }
            else if (card[0].Equals("cardLuchador"))
            {
                temp = new Luchador();
                makeCard(temp.cards[num]);
            }
            else if (card[0].Equals("cardBrawler"))
            {
                temp = new Brawler();
                makeCard(temp.cards[num]);
            }
            else if (card[0].Equals("cardGiant"))
            {
                temp = new Giant();
                makeCard(temp.cards[num]);
            }
            else if (card[0].Equals("cardTechnician"))
            {
                temp = new Technician();
                makeCard(temp.cards[num]);
            }

        }
        
    }

    public Card(string type, int attack, bool atk, bool toDown, bool pin, bool downOpp, 
                bool oppSkip, bool counter, bool oppDisc, bool draw, int numDraw, bool stand, 
                bool escape, bool submit, bool special, string name, int num)
    {
        this.type = type;
        this.attack = attack;
        this.atk = atk;
        this.toDown = toDown;
        this.pin = pin;
        this.downOpp = downOpp;
        this.oppSkip = oppSkip;
        this.counter = counter;
        this.oppDisc = oppDisc;
        this.draw = draw;
        this.numDraw = numDraw;
        this.stand = stand;
        this.escape = escape;
        this.submit = submit;
        this.special = special;
        this.name = name;
        this.num = num;
    }

    public void makeCard(Card card)
    {
        attack = card.attack;
        toDown = card.toDown;
        pin = card.pin;
        downOpp = card.downOpp;
        oppSkip = card.oppSkip;
        counter = card.counter;
        oppDisc = card.oppDisc;
        draw = card.draw;
        numDraw = card.numDraw;
        stand = card.stand;
        escape = card.escape;
        submit = card.submit;
        special = card.special;   
    }

   
    
    
}
