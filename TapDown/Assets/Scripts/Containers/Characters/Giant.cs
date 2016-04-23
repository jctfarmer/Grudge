public class Giant : Character
{

    public Giant() : base()
    {
        name = "giant";
        cardAtlas = "cardGiant";
        health = 30;
        refreshDamage = 2;
        standRate = 2;
        deck = new int[] { 0, 0, 1, 2, 2, 3, 3, 3, 3, 3, 4, 5, 5, 6, 6, 7, 7, 7, 7, 7, 8, 9, 9, 9, 9, 9, 9, 10,
                        11, 11, 11, 11, 12, 12, 13, 14, 15, 16, 17, 18, 18, 19, 20, 20, 20, 21, 22, 22, 23, 23};
        cards = new Card[] { new Card("attack", 3, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "big boot", 0),

                            new Card("submit", 0, true, false, false, false, false, false, false, false, 0,
                                        false, false, true, false, "bear hug", 1),

                            new Card("attack", 3, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "big leg drop", 2),

                            new Card("attack", 2, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "bodyslam", 3),

                            new Card("counter", 0, false, false, false, false, true, true, false, false, 0,
                                        false, false, false, false, "box the ears", 4),

                            new Card("counter", 3, false, false, false, true, false, true, false, false, 0,
                                        false, false, false, false, "catch and toss", 5),

                            new Card("attack", 4, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "chokeslam", 6),

                           new Card("counter", 0, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "counter", 7),

                            new Card("counter", 0, false, false, false, false, false, true, true, false, 0,
                                        false, false, false, false, "distraction", 8),

                            new Card("escape", 0, false, false, false, false, false, false, false, false, 0,
                                        false, true, false, false, "escape pin", 9),

                            new Card("pin", 2, true, false, true, true, false, false, false, false, 0,
                                        false, false, false, false, "falling bodyslam", 10),

                            new Card("attack", 2, true, false, false, false, false, false, false, false, 0,
                                        false, false, false, false, "giant clothesline", 11),

                            new Card("counter", 1, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "hard irish whip", 12),

                            new Card("counter", 0, false, false, false, false, false, true, false, true, 1,
                                        false, false, false, false, "ill intentions", 13),

                            new Card("counter", 0, false, false, false, false, false, true, false, true, 1,
                                        true, false, false, false, "imposing will", 14),

                             new Card("counter", 0, false, false, false, false, false, true, false, true, 2,
                                        true, false, false, false, "nefarious plans", 15),

                             new Card("pin", 0, true, false, true, false, false, false, false, false, 0,
                                        false, false, false, false, "pin cover", 16),

                             new Card("attack", 5, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, true, "powerbomb of doom", 17),

                            new Card("attack", 4, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "richter scale splash", 18),

                            new Card("counter", 0, false, false, false, true, false, true, false, false, 0,
                                        false, false, false, false, "shoulder block", 19),

                            new Card("attack", 2, true, false, false, false, true, false, false, false, 0,
                                        false, false, false, false, "slap across chest", 20),

                            new Card("pin", 0, true, false, true, false, false, false, false, false, 0,
                                        false, false, false, false, "splash slam", 21),

                            new Card("counter", 2, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "throat strike", 22),

                            new Card("attack", 2, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "walk across chest", 23),
                            };

    }
}