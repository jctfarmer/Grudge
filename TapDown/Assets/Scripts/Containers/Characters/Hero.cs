public class Hero : Character
{
    

    public Hero() : base()
    {
        name = "hero";
        cardAtlas = "cardHero";
        refreshDamage = 0;
        recoverGain = 2;
        deck = new int[] { 0, 0, 0, 1, 1, 2, 2, 2, 2, 2, 2, 3, 4, 5, 5, 6, 6, 6, 6, 6, 7, 8, 9, 9, 10, 11,
                        12, 12, 13, 13, 13, 13, 13, 14, 15, 16, 17, 18, 19, 19, 19, 19, 20, 21, 21, 21, 22, 23, 24, 25};
        


        cards = new Card[] { new Card("attack", 2, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "big boot", 0),

                            new Card("attack", 1, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "closeliner", 1),

                            new Card("counter", 0, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "counter", 2),

                            new Card("counter", 0, false, false, false, false, false, true, true, false, 0,
                                        false, false, false, false, "distraction", 3),

                            new Card("counter", 0, false, false, false, true, false, true, false, false, 0,
                                        false, false, false, false, "drop toe hold", 4),

                            new Card("counter", 0, false, false, false, false, false, true, false, false, 0,
                                        true, false, false, false, "endless courage", 5),

                            new Card("escape", 0, false, false, false, false, false, false, false, false, 0,
                                        false, true, false, false, "escape pin", 6),

                           new Card("attack", 5, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, true, "fate sealer a.k.a. the time is now for more trumpets", 7),

                            new Card("pin", 2, true, false, true, true, false, false, false, false, 0,
                                        false, false, false, false, "german suplex", 8),

                            new Card("counter", 1, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "hard irish whip", 9),

                            new Card("attack", 2, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "leg drop", 10),

                            new Card("pin", 0, true, false, true, false, false, false, false, false, 0,
                                        false, false, false, false, "pin cover", 11),

                            new Card("counter", 3, false, false, false, true, false, true, false, false, 0,
                                        false, false, false, false, "proto-slam", 12),

                            new Card("attack", 1, true, false, false, false, false, false, false, false, 0,
                                        false, false, false, false, "punch", 13),

                            new Card("counter", 0, false, false, false, false, false, true, false, true, 1,
                                        false, false, false, false, "respectfully declined", 14),

                            new Card("escape", 0, false, false, true, false, false, false, false, false, 0,
                                        false, true, false, false, "reversal pin", 15),

                            new Card("counter", 1, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "reversal suplex", 16),

                            new Card("attack", 3, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "running elbow drop", 17),

                            new Card("pin", 0, true, false, true, false, false, false, false, false, 0,
                                        false, false, false, false, "schoolboy roll-up", 18),

                            new Card("attack", 2, true, false, false, false, false, false, false, false, 0,
                                        false, false, false, false, "shoulder tackle", 19),

                            new Card("pin", 0, false, false, true, false, false, true, false, false, 0,
                                        false, false, false, false, "small package", 20),

                            new Card("attack", 0, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "snap mare", 21),

                            new Card("submit", 0, true, false, false, false, false, false, false, false, 0,
                                        false, false, true, false, "stf", 22),

                            new Card("attack", 2, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "superplex", 23),

                            new Card("counter", 0, false, false, false, false, false, true, false, true, 2,
                                        false, false, false, false, "the crowd", 24),

                            new Card("attack", 3, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "vertical suplex", 25)
                            };
    }
    
}
