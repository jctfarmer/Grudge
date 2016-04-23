public class Technician : Character
{
   
    public Technician() : base()
    {
        name = "technician";
        cardAtlas = "cardTechnician";
        subMod = 1;
        hand = new int[6] { -1, -1, -1, -1, -1, -1};
        deck = new int[] { 0, 0, 1, 1, 1, 1, 2, 2, 2, 3, 4, 5, 6, 6, 6, 6, 6, 7, 7, 8, 9, 10, 11, 11, 11, 11,
                        11, 12, 12, 12, 13, 14, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 25, 25, 26, 27, 28, 29};
        cards = new Card[] { new Card("pin", 0, false, false, false, false, false, true, false, false, 0,
                                        false, false, true, true, "ankle lock", 0),

                            new Card("attack", 1, true, false, false, false, false, false, false, false, 0,
                                        false, false, false, false, "arm drag", 1),

                            new Card("attack", 2, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "back suplex", 2),

                            new Card("counter", 0, false, false, false, false, true, true, false, false, 0,
                                        false, false, false, false, "box the ears", 3),

                            new Card("counter", 0, false, false, false, false, false, true, false, true, 2,
                                        false, false, false, false, "bradacious pushups", 4),

                            new Card("counter", 0, false, false, false, false, false, true, false, true, 1,
                                        false, false, false, false, "cartwheel", 5),
                            
                           new Card("counter", 0, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "counter", 6),

                           new Card("counter", 1, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "counter punch", 7),

                            new Card("pin", 0, true, false, true, false, false, false, false, false, 0,
                                        false, false, false, false, "crucifix pin", 8),

                             new Card("attack", 2, true, false, false, false, true, false, false, false, 0,
                                        false, false, false, false, "dragonscrew leg whip", 9),

                            new Card("counter", 0, false, false, false, true, false, true, false, false, 0,
                                        false, false, false, false, "drop toe hold", 10),

                            new Card("escape", 0, false, false, false, false, false, false, false, false, 0,
                                        false, true, false, false, "escape pin", 11),

                            new Card("attack", 2, true, false, false, false, false, false, false, false, 0,
                                        false, false, false, false, "european uppercut", 12),

                            new Card("attack", 1, true, false, false, false, true, false, false, false, 0,
                                        false, false, false, false, "eye gouge", 13),

                            new Card("attack", 1, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "headlock takedown", 14),

                            new Card("attack", 2, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "indian deathlock", 15),

                            new Card("pin", 2, true, false, true, true, false, false, false, false, 0,
                                        false, false, false, false, "medal stand", 16),

                            new Card("counter", 0, false, false, false, false, false, true, false, false, 0,
                                        true, false, false, false, "on your feet", 17),

                            new Card("attack", 3, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "overhead suplex", 18),

                            new Card("counter", 0, false, false, false, false, false, true, true, false, 0,
                                        false, false, false, false, "rake the eyes", 19),

                            new Card("escape", 0, false, false, true, false, false, false, false, false, 0,
                                        false, true, false, false, "reversal pin", 20),

                            new Card("submit", 0, true, false, false, false, false, false, false, false, 0,
                                        false, false, true, false, "semester abroad", 21),

                            new Card("submit", 0, true, false, false, false, false, false, false, false, 0,
                                        false, false, true, false, "sharpshooter", 22),

                            new Card("pin", 0, true, false, true, false, false, false, false, false, 0,
                                        false, false, false, false, "shoot the half gap", 23),

                            new Card("pin", 0, false, false, true, false, false, true, false, false, 0,
                                        false, false, false, false, "small package", 24),

                            new Card("attack", 0, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "snap mare", 25),

                            new Card("attack", 2, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "stomp to knee", 26),

                            new Card("attack", 4, true, false, false, true, false, false, false, false, 0,
                                        false, false, false, false, "superkick", 27),

                            new Card("attack", 2, true, true, false, false, false, false, false, false, 0,
                                        false, false, false, false, "top rope moonsault", 28),

                            new Card("counter", 2, false, false, false, false, false, true, false, false, 0,
                                        false, false, false, false, "tree of woe", 29)

                            };
    }
}
