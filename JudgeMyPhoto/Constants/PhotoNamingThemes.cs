using Marcware.JudgeMyPhoto.ExtensionMethods;

namespace Marcware.JudgeMyPhoto.Constants
{
    public static class PhotoNamingThemes
    {
        public static string GetThemeDescription(string themeCode)
        {
            switch (themeCode.EmptyIfNull())
            {
                case ThemeCodes.AmericanStateCapitals:
                    return "American state capitals";
                case ThemeCodes.BritishPrimeMinisters:
                    return "British Prime Ministers";
                case ThemeCodes.Explorers:
                    return "Explorers";
                case ThemeCodes.FrenchProvinces:
                    return "French provinces";
                case ThemeCodes.Fruit:
                    return "Fruit";
                case "":
                    return "Not defined";
                default:
                    return "Unknown";
            }
        }

        public static string[] GetThemeItems(string themeCode)
        {
            switch (themeCode.EmptyIfNull())
            {
                case ThemeCodes.AmericanStateCapitals:
                    return ThemeItems.AmericanStateCapitals;
                case ThemeCodes.BritishPrimeMinisters:
                    return ThemeItems.BritishPrimeMinisters;
                case ThemeCodes.Explorers:
                    return ThemeItems.Explorers;
                case ThemeCodes.FrenchProvinces:
                    return ThemeItems.FrenchProvinces;
                case ThemeCodes.Fruit:
                    return ThemeItems.Fruit;
                default:
                    return new string[] { };
            }
}

        public static class ThemeCodes
        {
            public const string Explorers = "EX";
            public const string FrenchProvinces = "FP";
            public const string BritishPrimeMinisters = "BPM";
            public const string AmericanStateCapitals = "ASC";
            public const string Fruit = "FR";

            public static string[] GetAll()
            {
                return new string[] { AmericanStateCapitals, BritishPrimeMinisters, Explorers, FrenchProvinces, Fruit };
            }
        }

        public static class ThemeItems
        {
            public readonly static string[] Explorers = { "Aldrin", "Allen", "Bell", "Amundsen", "Burton", "Cabot", "Cabral", "Clapperton", "Columbus", "Cook", "Cortes", "da Gama", "Dampier", "Drake", "Elcano", "Fiennes", "Flinders", "Henry", "Hudson", "Kingsley", "Livingstone", "Mackenzie", "Magellan", "Park", "Polo", "Raleigh", "Ross", "Scott", "Shackleton", "Stark", "Vancouver", "Vespucci", "Zheng He" };
            public readonly static string[] FrenchProvinces = { "Île-de-France", "Berry", "Orleanais", "Normandy", "Languedoc", "Lyonnais", "Dauphine", "Champagne", "Aunis", "Saintonge", "Poitou", "Guyenne", "Gascony", "Burgundy", "Picardy", "Anjou", "Provence", "Angoumois", "Bourbonnais", "Marche", "Brittany", "Maine", "Touraine", "Limousin", "Foix", "Auvergne", "Bearn", "Alsace", "Artois", "Roussillon", "Flanders", "Hainaut", "Franche-Comte", "Lorraine", "Barrois", "Corsica", "Nivernais" };
            public readonly static string[] BritishPrimeMinisters = { "Johnson", "May", "Cameron", "Brown", "Blair", "Major", "Thatcher", "Callaghan", "Wilson", "Heath", "Wilson", "Douglas-Home", "Macmillan", "Eden", "Churchill", "Attlee", "Chamberlain", "Baldwin", "MacDonald", "Bonar Law", "Lloyd George", "Asquith", "Campbell-Bannerman", "Balfour", "Gascoyne-Cecil", "Primrose", "Gladstone", "Disraeli", "Smith-Stanley", "Russell", "Temple", "Smith-Stanley", "Hamilton-Gordon", "Peel", "Lamb", "Wellesley", "Grey", "Robinson", "Canning", "Jenkinson", "Perceval", "Cavendish-Bentinck", "Grenville", "Pitt", "Addington", "Petty", "Watson-Wentworth", "North", "FizRoy", "Stuart", "Cavendish", "Pelham-Holles", "Pelham", "Compton", "Walpole" };
            public readonly static string[] AmericanStateCapitals = { "Montgomery", "Juneau", "Phoenix", "Little Rock", "Sacramento", "Denver", "Hartford", "Dover", "Tallahassee", "Atlanta", "Honolulu", "Boise", "Springfield", "Indianapolis", "Des Moines", "Topeka", "Frankfort", "Baton Rouge", "Augusta", "Annapolis", "Boston", "Lansing", "Saint Paul", "Jackson", "Jefferson City", "Helena", "Lincoln", "Carson City", "Concord", "Trenton", "Santa Fe", "Albany", "Raleigh", "Bismark", "Columbus", "Oklahoma", "Salem", "Harrisburg", "Providence", "Columbia", "Pierre", "Nashville", "Austin", "Salt Lake City", "Montpellier", "Richmond", "Olympia", "Charleston", "Madison", "Cheyenne" };
            public readonly static string[] Fruit = { "Apple", "Banana", "Cherry", "Dorian", "Elderberry", "Fig", "Grape", "Guava", "Honeydew Melon", "Indian Prune", "Jackfruit", "Kiwi", "Lime", "Lemon", "Mango", "Nectarine", "Orange", "Papaya", "Pear", "Quince", "Raspberry", "Strawberry", "Tomato", "Tangerine", "Ugli Fruit", "Vanilla Bean", "Watermelon", "Xigua", "Yangmei", "Zig Zag" };
        }
    }
}
