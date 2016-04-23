public class Brawler : Character {
    
    public Brawler() : base()
    {
        name = "brawler";
        cardAtlas = "cardBrawler";
        health = 25;
        damageMod = 1;
        counterCounter = false;
        deck = new int[] { 0, 0, 0, 1, 2, 3, 3, 3, 3, 4, 5, 6, 6, 7, 7, 7, 7, 7, 8, 9, 9, 9, 9, 9, 10, 10,
                        11, 12, 12, 13, 14, 15, 16, 17, 18, 18, 19, 20, 21, 21, 21, 22, 23, 24, 25, 26, 27, 28, 28, 29};
        cards = new Card[] { new Card("attack", 2, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "alabama slam", 0),

                            new Card("attack", 3, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "atomic drop", 1),

                            new Card("attack", 1, true, false, false, false, true, false, false, false, 0,
                                        false, false, false, false, "blatent choke", 2),

                            new Card("attack", 1, true, false, false, false, false, false, false, false, 0,
                                        false, false, false, false, "body punch", 3),

                            new Card("counter", 0, false, false, false, true, false, true, false, false, 0,
                                        false, false, false, false, "counter", 4),

                            new Card("counter", 0, false, false, false, false, false, true, true, false, 0,
                                        false, false, false, false, "clock cleaning", 5),

                            new Card("attack", 3, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "closeliner", 6),

                            new Card("counter", 0, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "counter", 7),

                            new Card("counter", 0, false, false, false, false, true, true, false, false, 0,
                                        false, false, false, false, "ear biting", 8),

                            new Card("escape", 0, false, false, false, false, false, false, false, false, 0,
                                        false, true, false, false, "escape pin", 9),

                            new Card("counter", 1, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "hard irish whip", 10),
                            
                           new Card("attack", 5, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, true, "high noon", 11),

                           new Card("attack", 1, true, false, false, false, true, false, false, false, 0,
                                        false, false, false, false, "kick to groin", 12),

                            new Card("attack", 2, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "knee drop", 13),

                            new Card("counter", 0, false, false, false, false, false, true, false, true, 1,
                                        false, false, false, false, "last call", 14),

                            new Card("attack", 2, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "mudhole stomping", 15),

                            new Card("counter", 0, false, false, false, false, false, true, false, false, 0,
                                        true, false, false, false, "on your feet", 16),

                            new Card("pin", 0, true, false, true, false, false, false, false, false, 0,
                                        false, false, false, false, "pin cover", 17),

                            new Card("attack", 0, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "push down automata", 18),

                            new Card("escape", 0, false, false, true, false, false, false, false, false, 0,
                                        false, true, false, false, "reversal pin", 19),

                            new Card("pin", 0, true, false, true, false, false, false, false, false, 0,
                                        false, false, false, false, "schoolboy roll-up", 20),

                            new Card("attack", 1, true, false, false, false, false, false, false, false, 0,
                                        false, false, false, false, "short arm clothesline", 21),

                            new Card("submit", 0, true, false, false, false, false, false, false, false, 0,
                                        false, false, true, false, "sleeperhold", 22),

                            new Card("pin", 0, false, false, true, false, false, true, false, false, 0,
                                        false, false, false, false, "small package", 23),

                            new Card("counter", 2, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "spinning side slam", 24),

                            new Card("attack", 2, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "step on face", 25),

                            new Card("counter", 0, false, false, false, false, false, true, false, true, 2,
                                        false, false, false, false, "sunset overdrive ride", 26),

                            new Card("attack", 2, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "superman uppercut", 27),

                            new Card("counter", 3, false, false, false, true, false, true, false, false, 0,
                                        false, false, false, false, "spinning side slam", 28),

                            new Card("attack", 0, true, false, false, false, true, false, false, false, 0,
                                        false, false, false, false, "thumb to the eye", 27)
                            };
    }
}
