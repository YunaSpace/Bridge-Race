namespace YunaSpace.BridgeRace
{
    [System.Serializable]
    public class TeamRank
    {
        public ColorType[] Winners => winners;
        public ColorType First => winners[0];
        public ColorType Second => winners[1];
        public ColorType Third => winners[2];

        private ColorType[] winners = new ColorType[3];

        public TeamRank(ColorType first, ColorType second, ColorType third)
        {
            winners[0] = first;
            winners[1] = second;
            winners[2] = third;
        }
    }
}
