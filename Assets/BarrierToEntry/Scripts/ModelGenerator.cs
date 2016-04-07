using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class ModelGenerator : MonoBehaviour
    {
        // These are all super rough eyeballed approximations.

        public static readonly Color HAIR_LIGHT_BROWN = ModelDesigner.rgb(140, 104, 74);    // Accepted. Not tested for approval.
        public static readonly Color HAIR_BROWN = ModelDesigner.rgb(102, 79, 60);           // Accepted. Not tested for approval.
        public static readonly Color HAIR_DARK_BROWN = ModelDesigner.rgb(51, 42, 34);       // Accepted. Not tested for approval.
        public static readonly Color HAIR_BLONDE = ModelDesigner.rgb(228, 171, 57);         // Accepted. Not tested for approval.
        public static readonly Color HAIR_DIRTY_BLONDE = ModelDesigner.rgb(185, 138, 45);   // Accepted. Not tested for approval.
        public static readonly Color HAIR_GINGER = ModelDesigner.rgb(216, 79, 18);      // Accepted by Haley. No other approval needed.
        public static readonly Color HAIR_WHITE = ModelDesigner.rgb(210, 210, 210);     // Accepted by Haley. No other approval needed.
        public static readonly Color HAIR_GREY = Color.gray;                            // Accepted by Haley. No other approval needed.
        public static readonly Color HAIR_BLACK = ModelDesigner.rgb(0, 0, 0);           // Accepted by Haley. No other approval needed.
        public static readonly Color HAIR_DEFAULT = ModelDesigner.rgb(105, 65, 4);      // Default. No approval needed.

        private static readonly Color[] hairColors = new Color[] { HAIR_DEFAULT, HAIR_LIGHT_BROWN, HAIR_BROWN, HAIR_DARK_BROWN, HAIR_BLONDE, HAIR_DIRTY_BLONDE, HAIR_GINGER, HAIR_WHITE, HAIR_GREY, HAIR_BLACK };

        public static readonly Color EYES_WHITE = Color.white;                              // Accepted by Haley. No other approval needed.
        public static readonly Color EYES_DEMON = Color.black;                              // Accepted by Haley. No other approval needed.
        public static readonly Color EYES_LIGHT_BLUE = ModelDesigner.rgb(152, 255, 249);    // Accepted by Haley. No other approval needed.
        public static readonly Color EYES_LIGHT_GREEN = ModelDesigner.rgb(190, 255, 219);   // Accepted by Haley. No other approval needed.
        public static readonly Color EYES_DEFAULT = Color.gray;                             // Default. No approval needed.

        private static readonly Color[] eyeColors = new Color[] { EYES_DEFAULT, EYES_WHITE, EYES_LIGHT_GREEN, EYES_LIGHT_BLUE, EYES_DEMON };

        public static readonly Color CLOTHES_WHITE = Color.white;                                   // Accepted. Not tested for approval.
        public static readonly Color CLOTHES_GREY = Color.gray;                                     // Accepted. Not tested for approval.
        public static readonly Color CLOTHES_BLACK = Color.black;                                   // Accepted. Not tested for approval.
        public static readonly Color CLOTHES_LIGHT_BLUE = ModelDesigner.rgb(62, 128, 255);          // Accepted. Not tested for approval.
        public static readonly Color CLOTHES_LIGHT_RED = ModelDesigner.rgb(255, 54, 54);            // Accepted. Not tested for approval.
        public static readonly Color CLOTHES_DARK_BLUE = ModelDesigner.rgb(0, 11, 120);             // Accepted. Not tested for approval.
        public static readonly Color CLOTHES_DARK_RED = ModelDesigner.rgb(68, 0, 0);                // Accepted. Not tested for approval.
        public static readonly Color CLOTHES_FLAMING_ORANGE = ModelDesigner.rgb(255, 41, 0);        // Accepted. Not tested for approval.
        public static readonly Color CLOTHES_LEMON_YELLOW = ModelDesigner.rgb(184, 210, 0);         // Accepted. Not tested for approval.
        public static readonly Color CLOTHES_LIME_GREEN = ModelDesigner.rgb(138, 255, 85);          // Accepted by Haley. No other approval needed.
        public static readonly Color CLOTHES_PURPLE_PEOPLE_EATER = ModelDesigner.rgb(71, 18, 163);  // Accepted by Haley. No other approval needed.
        public static readonly Color CLOTHES_SULLY_TEAL = ModelDesigner.rgb(0, 179, 179);           // Accepted by Haley. No other approval needed.
        public static readonly Color CLOTHES_MAGIC_MAGENTA = ModelDesigner.rgb(153, 0, 77);         // Accepted by Haley. No other approval needed.
        public static readonly Color CLOTHES_DEFAULT = Color.white;                                 // Default. No approval needed.

        public static readonly Color[] clothesColors = new Color[] {
            CLOTHES_DEFAULT, CLOTHES_WHITE, CLOTHES_GREY, CLOTHES_BLACK, CLOTHES_MAGIC_MAGENTA, CLOTHES_LIGHT_RED,
            CLOTHES_DARK_RED, CLOTHES_FLAMING_ORANGE, CLOTHES_LEMON_YELLOW, CLOTHES_LIME_GREEN, CLOTHES_SULLY_TEAL,
            CLOTHES_LIGHT_BLUE, CLOTHES_DARK_BLUE, CLOTHES_PURPLE_PEOPLE_EATER
        };

        public static readonly Color EQUIPMENT_LIGHT_GREY = ModelDesigner.rgb(222, 222, 222);
        public static readonly Color EQUIPMENT_DARK_GREY = ModelDesigner.rgb(65, 65, 65);
        public static readonly Color EQUIPMENT_DEFAULT = Color.gray;
        public static readonly Color EQUIPMENT_APPLE_LOVER = Color.white;
        public static readonly Color EQUIPMENT_WINDOWS_LOVER = Color.black;


        private static readonly Color[] equipmentColors = new Color[] { EQUIPMENT_APPLE_LOVER, EQUIPMENT_LIGHT_GREY, EQUIPMENT_DEFAULT, EQUIPMENT_DARK_GREY, EQUIPMENT_WINDOWS_LOVER };

        public static readonly Color SKIN_TAN = ModelDesigner.rgb(255, 224, 189);                   // Accepted. Not tested for approval.
        public static readonly Color SKIN_BROWN = ModelDesigner.rgb(77, 77, 77);                    // Accepted. Not tested for approval.
        public static readonly Color SKIN_PALE_WHITE = Color.white;                                 // This is as pale as I can possibly make it. Approval not applicable.
        public static readonly Color SKIN_SUNBURN = ModelDesigner.rgb(255, 159, 159);               // Accepted. Not tested for approval.
        public static readonly Color SKIN_ATE_TOO_MUCH_SILVER = ModelDesigner.rgb(159, 164, 255);   // Accepted. Not tested for approval.
        public static readonly Color SKIN_LADY_LIBERTY = ModelDesigner.rgb(112, 210, 190);          // Accepted. Not tested for approval.
        public static readonly Color SKIN_DEFAULT = Color.gray;
        // Asian and hispanic skin removed due to difficulty in layered skin colors

        private static readonly Color[] skinColors = new Color[] { SKIN_PALE_WHITE, SKIN_TAN, SKIN_DEFAULT, SKIN_BROWN, SKIN_SUNBURN, SKIN_LADY_LIBERTY, SKIN_ATE_TOO_MUCH_SILVER };
        
        public static readonly Color BEAM_BLUE = ModelDesigner.rgb(20, 105, 218);
        public static readonly Color BEAM_RED = Color.red;
        public static readonly Color BEAM_YELLOW = Color.yellow;
        public static readonly Color BEAM_PINK = ModelDesigner.rgb(255, 0, 127);
        public static readonly Color BEAM_YODA = Color.green;
        public static readonly Color BEAM_ORANGE = ModelDesigner.rgb(255, 128, 0);
        public static readonly Color BEAM_PURPLE = ModelDesigner.rgb(102, 0, 204);

        private static readonly Color[] beamColors = new Color[] { BEAM_PINK, BEAM_RED, BEAM_ORANGE, BEAM_YELLOW, BEAM_YODA, BEAM_BLUE, BEAM_PURPLE };

        public static Texture DEFAULT_SCIENTIST;
        public static Texture SCIFI_BLACK;
        public static Texture DARKEST_BLACK;
        public static Texture LIGHTER_BLACK;

        public Texture def_scientist;
        public Texture scifi_black;
        public Texture dark_black;
        public Texture light_black;

        private static Texture[] clothesTextures;

        public static void RandomizeModel(Actor actor)
        {
            ModelDesigner design = actor.modelDesign;
            design.SetColor(BodyPart.HAIR, RandomColor(hairColors));
            design.SetColor(BodyPart.EYES, RandomColor(eyeColors));
            design.SetColor(BodyPart.CLOTHES, RandomColor(clothesColors));
            design.SetColor(BodyPart.EQUIPMENT, RandomColor(equipmentColors));
            design.SetColor(BodyPart.SKIN, RandomColor(skinColors));
            design.SetColor(BodyPart.BEAM, RandomColor(beamColors));
            design.SetBald(Random.value > 0.7f);
            design.SetClothing(RandomTexture());
        }

        public static Color RandomColor(Color[] colors)
        {
            return colors[Mathf.RoundToInt(Random.Range(0, colors.Length-1))];
        }

        public static Texture RandomTexture()
        {
            return clothesTextures[Mathf.RoundToInt(Random.Range(0, clothesTextures.Length - 1))];
        }

        void Start()
        {
            DEFAULT_SCIENTIST = def_scientist;
            SCIFI_BLACK = scifi_black;
            DARKEST_BLACK = dark_black;
            LIGHTER_BLACK = light_black;
            clothesTextures = new Texture[] { DEFAULT_SCIENTIST, SCIFI_BLACK, DARKEST_BLACK, LIGHTER_BLACK };
        }
    }
}
