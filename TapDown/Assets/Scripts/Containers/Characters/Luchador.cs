public class Luchador : Character
{
   

    public Luchador() : base()
    {
        name = "luchador";
        cardAtlas = "cardLuchador";
        health = 15;
        refreshDamage = -1;
        turnCounter = 2;
        deck = new int[] { 0, 1, 2, 2, 2, 2, 2, 3, 4, 5, 5, 6, 7, 8, 8, 8, 8, 8, 9, 10, 11, 11, 11, 12, 12,
                        13, 13, 14, 15, 16, 16, 16, 16, 16, 17, 17, 17, 18, 19, 20, 21, 21, 22, 22, 23, 24, 24, 24, 25, 26};

        cards = new Card[] { new Card("attack", 5, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, true, "450", 0),

                            new Card("attack", 3, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "backstabber", 1),

                            new Card("counter", 0, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "counter", 2),

                            new Card("counter", 0, false, false, false, false, false, true, true, false, 0,
                                        false, false, false, false, "distraction", 3),

                            new Card("attack", 4, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "diving senton splash", 4),

                            new Card("attack", 1, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "dropkick to knee", 5),

                            new Card("counter", 0, false, false, false, true, false, true, false, false, 0,
                                        false, false, false, false, "drop toe hold", 6),

                            new Card("attack", 4, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "enziguri", 7),

                          new Card("escape", 0, false, false, false, false, false, false, false, false, 0,
                                        false, true, false, false, "escape pin", 8),

                            new Card("submit", 0, true, false, false, false, false, false, false, false, 0,
                                        false, false, true, false, "flying armbar", 9),

                            new Card("counter", 2, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "flying ddt counter", 10),

                            new Card("attack", 2, true, false, false, false, false, false, false, false, 0,
                                        false, false, false, false, "flying head scissors", 11),

                            new Card("counter", 1, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "hard irish whip", 12),

                            new Card("pin", 2, true, false, true, true, false, false, false, false, 0,
                                        false, false, false, false, "hurricanranna pin", 13),

                            new Card("attack", 2, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "knee drop", 14),

                            new Card("pin", 0, true, false, true, false, false, false, false, false, 0,
                                        false, false, false, false, "la magistral cradle", 15),

                            new Card("attack", 1, true, false, false, false, false, false, false, false, 0,
                                        false, false, false, false, "leg kick", 16),

                            new Card("attack", 0, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "snap mare", 17),

                            new Card("counter", 0, false, false, false, false, false, true, false, true, 1,
                                        false, false, false, false, "ole ole ole", 18),

                            new Card("counter", 0, false, false, false, false, false, true, false, false, 0,
                                        true, false, false, false, "one your feet", 19),

                            new Card("escape", 0, false, false, true, false, false, false, false, false, 0,
                                        false, true, false, false, "reversal pin", 20),

                            new Card("counter", 1, false, false, false, true, false, true, false, false, 0,
                                        false, false, false, false, "reverse hurricanrana", 21),

                            new Card("pin", 0, false, false, true, false, false, true, false, false, 0,
                                        false, false, false, false, "small package", 22),

                            new Card("attack", 3, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "leg drop", 23),

                            new Card("attack", 2, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "tilt-a-whirl backbreaker", 24),

                            new Card("pin", 0, true, false, true, false, false, false, false, false, 0,
                                        false, false, false, false, "victory roll", 25),

                            new Card("counter", 0, false, false, false, false, false, true, false, true, 22,
                                        false, false, false, false, "!viva la luchador!", 26),
                            };
    }
}
