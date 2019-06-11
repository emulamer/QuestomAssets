using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.BeatSaber
{
    public static class BSConst
    {
        public static class KnownFiles
        {
            //public const string File19 = "sharedassets19.assets";
            //public const string File17 = "sharedassets17.assets";
            //public const string File11 = "sharedassets11.assets";
            //public const string File14 = "sharedassets14.assets";
            //public const string File1 = "sharedassets1.assets";

            //public const string FullFile19Path = AssetsRootPath + File19;
            //public const string FullFile17Path = AssetsRootPath + File17;
            //public const string FullFile11Path = AssetsRootPath + File11;
            //public const string FullFile14Path = AssetsRootPath + File14;

            public const string AssetsRootPath = "assets/bin/Data/";
            //public const string SaberAssetsFilename = File11;
            //public const string SongsAssetsFilename = File17;
            //public const string MainCollectionAssetsFilename = File19;
            //public const string ColorAssetsFilename = File1;
            //public const string TextAssetFilename = "231368cb9c1d5dd43988f2a85226e7d7";
            

            //public const string FullSaberAssetsPath = AssetsRootPath + SaberAssetsFilename;
            //public const string FullSongsAssetsPath = AssetsRootPath + SongsAssetsFilename;
            //public const string FullMainCollectionAssetsPath = AssetsRootPath + MainCollectionAssetsFilename;
        }

        public static class Colors
        {
            // Could use a hex string to calculate this exactly, but I was too lazy
            public static readonly Color DefaultColorA = new Color()
            {
                R = 0.188235f,
                G = 0.619608f,
                B = 1.0f,
                A = 1.0f
            };
            public static readonly Color DefaultColorB = new Color()
            {
                R = 0.941176f,
                G = 0.188235f,
                B = 0.188235f,
                A = 1.0f
            };
        }

        public static class NameSuffixes
        {
            public const string LevelPack = "LevelPack";
            public const string LevelCollection = "LevelPackCollection";
            public const string Level = "Level";
            public const string PackCover = "Cover";
        }

        public static class ScriptHash
        {
            //public static Guid BeatmapLevelPackScriptHash { get { return new Guid("252e448f-a4c9-c8aa-dabe-c88917b0dc7d"); } }
            //public static Guid BeatmapLevelCollectionScriptHash { get { return new Guid("59dd0c93-dbc2-fac4-6745-01914a570ac2"); } }
            //public static Guid MainLevelsCollectionHash { get { return new Guid("8398a1c6-7d3b-cc41-e8d7-83cd6a11bfd4"); } }
            //public static Guid BeatmapLevelDataHash { get { return new Guid("4690eca3-1201-f506-cd10-9314850602e3"); } }
            //public static Guid BeatmapDataHash { get { return new Guid("8d3caf95-6f40-5cf3-9da1-51e0ee1e0013"); } }

        }

        


        public static Dictionary<string, Type> GetAssetTypeMap()
        {
            Dictionary<string, Type> scriptHashToTypes = new Dictionary<string, Type>();
            scriptHashToTypes.Add("BeatmapLevelPackSO", typeof(BeatmapLevelPackObject));
            scriptHashToTypes.Add("BeatmapLevelCollectionSO", typeof(BeatmapLevelCollectionObject));
            scriptHashToTypes.Add("BeatmapLevelPackCollectionSO", typeof(MainLevelPackCollectionObject));
            scriptHashToTypes.Add("BeatmapDataSO", typeof(BeatmapDataObject));
            scriptHashToTypes.Add("BeatmapLevelSO", typeof(BeatmapLevelDataObject));
            scriptHashToTypes.Add("SimpleColorSO", typeof(SimpleColorSO));
            scriptHashToTypes.Add("ColorManager", typeof(ColorManager));
            scriptHashToTypes.Add("AlwaysOwnedContentModelSO", typeof(AlwaysOwnedContentModel));
            return scriptHashToTypes;
        }

        public static List<string> KnownLevelPackIDs { get; } = new List<string>() {
            "OstVol1",
            "OstVol2",
            "Extras",
            "Monstercat",
            "ImagineDragons"
        };

        public static List<string> KnownLevelIDs { get; } = new List<string>()
        {
            "100BillsLevel",
            "AngelVoices",
            "BeatSaber",
            "BalearicPumping",
            "BeThereForYou",
            "Boundless",
            "Breezer",
            "CommercialPumping",
            "CountryRounds",
            "CrabRave",
            "Elixia",
            "EmojiVIP",
            "Epic",
            "Escape",
            "FeelingStronger",
            "INeedYou",
            "Legend",
            "LvlInsane",
            "Metronome",
            "OneHope",
            "Overkill",
            "PopStars",
            "Rattlesnake",
            "RumNBass",
            "Escape",
            "Stronger",
            "ThisTime",
            "TillItsOver",
            "TurnMeOn",
            "UnlimitedPower",
            "WeWontBeAlone",
            "BadLiar",
            "Believer",
            "Digital",
            "ItsTime",
            "Machine",
            "Natural",
            "Radioactive",
            "Thunder",
            "Warriors",
            "WhateverItTakes"
        };

        public const string DebugCertificatePEM = @"-----BEGIN CERTIFICATE-----
MIICpjCCAY6gAwIBAgIITpLEzAv/BWIwDQYJKoZIhvcNAQELBQAwEjEQMA4GA1UE
AwwHVW5rbm93bjAgFw0wOTA2MDIwMDAwMDBaGA8yMDY5MDYwMjAwMDAwMFowEjEQ
MA4GA1UEAwwHVW5rbm93bjCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEB
AKTjqOckhu7QSfheDcFOtMmq3oYagrybDyIvUkQQfD5bN03dGq+3eD4N5OgZTip5
+W3WCWZCqQESwb2spb9Wx7QLYOeZb8FXlGIwo5d6nvRFHKm4Bomr37t0NcSK+JRD
a3/MOgPP5KQJ5L/z3RCZBKxn0zZBcrUrBLI/0z6kFFCmIo9b/TDQf8Si+mCeM8fu
dH32TTPVUk1mrhssOkykhsxCPbpHzZIj3TKGk04g2es1SlIEgQIldWswa4xkTjny
C7pi3hhpQuLKUpYO2GHhT5aq4J2rpZVScEzLiNckM9iyC+9MdWyG++hlrSb9GeAn
rwqiHN9BjYt8BtvpEDGahMcCAwEAATANBgkqhkiG9w0BAQsFAAOCAQEAAzWf0UuC
ZK7UWnyXltiAqmIHGduEVNaU8gQHvYlS7UiYWgieC2MhYcpojIWf78/n6TP46xUj
Zcs2WHw4M76ppp4Z0t32T4wKMV64rvxmxrT1rnrocpalHEW0L7o6npPwQdin58kY
ip+5dNleQmeFy8E/Plew3E3JiQKedfIR9xj3BNFr4cZHhuIk8bMXi8v4p7dr6A+4
cCYOowy93Oirb1z9RBQqaPQZkQWVH+LaRQ95CMu688hksVXUZz6ZcRzxtQsMmKj3
r/4yonSyufkTY2Sky0myL04/gbDCqLSi1CLo0ksFSRi7d9oChCtNewNoXByGq09X
09SW0xPRYHxoSA==
-----END CERTIFICATE-----
-----BEGIN RSA PRIVATE KEY-----
MIIEogIBAAKCAQEApOOo5ySG7tBJ+F4NwU60yarehhqCvJsPIi9SRBB8Pls3Td0a
r7d4Pg3k6BlOKnn5bdYJZkKpARLBvaylv1bHtAtg55lvwVeUYjCjl3qe9EUcqbgG
iavfu3Q1xIr4lENrf8w6A8/kpAnkv/PdEJkErGfTNkFytSsEsj/TPqQUUKYij1v9
MNB/xKL6YJ4zx+50ffZNM9VSTWauGyw6TKSGzEI9ukfNkiPdMoaTTiDZ6zVKUgSB
AiV1azBrjGROOfILumLeGGlC4spSlg7YYeFPlqrgnaullVJwTMuI1yQz2LIL70x1
bIb76GWtJv0Z4CevCqIc30GNi3wG2+kQMZqExwIDAQABAoIBAC0gYUlhJcyWFKh0
lS8iazgGG4B4IO+dQDcK3GjkWhx2ulwE9xjADZhuFQewZUQavbjhqxDhjX9NsthG
N9Z12ZHcy1iXFY7EeUemKB9836Pahk2sn51t/H1BALYZko6BJRqEuhvw+ZIrYv9l
rkqslirY/2UJ5GrQqyhdb2LlZOntHFDwYesZsKxj0v+IV4P9eRcwtEYr2M1AzZPb
qIEx1v+P6DpZsWDViUpzkfcjqYziViEeXIgkjeaQYkzCwn7h/iTT6WC/VUrMTGzB
mZ4wsDinrMTLQBYqaafmX7Ff172u8D7fIyjTLJjx9mbG3hWSJPfbPl1lDaw+tWrW
aoyZXgECgYEA7MHb8FM5xksv2UEpEDJY9IZbIHWNk+sGZv13bwP5AxMJYfWy0wpe
hGoqc49TSwG8EZwLhfvAtA0BXDXLI3+biulnO3JU6hgAeSzQyk3RQFjB08QM7CbQ
Uzb3T2pRgjRJdFgrRlUBtj19VC2sTR/zUqVSSxAniKGMRQ4zoIToINECgYEAskp2
3/EQDEj4u64Ggi5KaJtvKiITO9tGFt0svSuFCCgcM4sBg6EFX6JEbMfbepRZ5GTW
h9W8XLo9jIZBQYqLhuKJtJ+PlEBgAxK4JAEwgqmWTcHBXdYJicFVJMLsmQOK4HiJ
hNgsIaQTvyhl9/aNgJA96wjUb//pVsqSMNSa8hcCgYBxoUFMAMWz1BYs8UciDOgA
xBMsav7+RUiXWYNe9ssmnJZeO6wN+eYPK10ghWN2lmiLExe8wG1mfO9wMClE6lPe
wdLYBzGWANsJTWcQEXUiqvasCmYhWSeXKMRdiyt/kFTI0CBE6zudGbnzEtClW3ZO
7iWm/SPcQZyu7/f7TI6UYQKBgAGYzyXEV/t0L94meeJynbIAKme7NGbl2OPdiUgM
er2O9mmzxgiyyYSIxIog5CNd7swv5wgCbxR5ipGWpkD7B7LmlosqnrOaPAHrCgEw
jYmuES2THbNEdoNoWuXgZRQdxwGpsrmg4gxPFuowZ3FoIO5U3GkdhCGYrjNbzyFm
1hhzAoGAV6iFwnriGgTLQEz4Pjviqq05SrS2+6jvP6siB9I7GBPlSIQSBMPdyCKA
8hebhfmRmEjRzPxqxKAE3d68MIeZ3n5g0IFcPL+ps3u937qmsttKWgubjkBTr2Ot
hEJ9cirq8PX32lYS3Q5lHaFjlzNgVvijDQCFuxA4NOj+hDFfC/Q=
-----END RSA PRIVATE KEY-----";
    }
}
