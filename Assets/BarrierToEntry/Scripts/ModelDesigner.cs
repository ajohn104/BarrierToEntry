using UnityEngine;
using System.Collections;

namespace BarrierToEntry {
    public enum BodyPart { HAIR, EYES, CLOTHES, EQUIPMENT, SKIN, BEAM }

    public class ModelDesigner : MonoBehaviour {

        public Renderer bodyRenderer;
        public Renderer hairRenderer;
        public Renderer beamRenderer;
        public Renderer beamCapRenderer;
        public new Light light;

        private Material eyes;
        private Material hair;
        private Material clothes;
        private Material skin;
        private Material equipment;
        private Material beam;
        private Material beamCap;
        private bool LockedIn = false;

        public void LockIn(bool Locked = true)
        {
            LockedIn = Locked;
        }

        public void Prepare() {
            Material[] mats = bodyRenderer.materials;
            foreach (Material material in mats)
            {
                if (material.name.Contains("CharacterScientistEyes")) eyes = material;
                else if (material.name.Contains("CharacterScientistClothes")) clothes = material; 
                else if (material.name.Contains("CharacterScientistSkin")) skin = material;
                else if (material.name.Contains("CharacterScientistEquipment")) equipment = material;
            }
            hair = hairRenderer.material;
            beam = beamRenderer.material;
            beamCap = beamCapRenderer.material;
        }

        public static Color rgb(float r, float g, float b)
        {
            return new Color(r / 255f, g / 255f, b / 255f);
        }

        private Material GetMat(BodyPart part)
        {
            
            switch (part)
            {
                case BodyPart.HAIR:
                    return hair;
                case BodyPart.EYES:
                    return eyes;
                case BodyPart.CLOTHES:
                    return clothes;
                case BodyPart.EQUIPMENT:
                    return equipment;
                case BodyPart.SKIN:
                    return skin;
                case BodyPart.BEAM:
                    return beam;
                default: return null;
            }
        }

        /// <summary>
        /// Sets the rgb color of a specific part of this instance of the character.
        /// </summary>
        /// <param name="part">The part of the character you wish to edit</param>
        /// <param name="r">Ranges 0 to 255</param>
        /// <param name="g">Ranges 0 to 255</param>
        /// <param name="b">Ranges 0 to 255</param>
        public void SetColor(BodyPart part, int r, int g, int b)
        {
            SetColor(part, rgb(r, g, b));
        }

        /// <summary>
        /// Sets the rgb color of a specific part of this instance of the character.
        /// </summary>
        /// <param name="part">The part of the character you wish to edit</param>
        /// <param name="col">The color to which the part will be set</param>
        public void SetColor(BodyPart part, Color col)
        {
            if (part == BodyPart.BEAM)
            {
                if (LockedIn) return;
                float hue = 0f;
                float sat = 0f;
                float val = 0f;
                
                Vector3 hsv = RGBtoHSV(col);
                hue = hsv[0];
                sat = hsv[1];
                val = hsv[2];
                Color rgbHDR = HSVToRGB(hue, 0.7720589f, 5f, true);
                beam.SetColor("_EmissionColor", rgbHDR);
                beamCap.SetColor("_EmissionColor", rgbHDR);
                light.color = col;
            }
            else {
                GetMat(part).color = col;
            }
        }

        /// <summary>
        /// Converts between RGB and HSV color values. Used to relax necessity for Unity 5.3 to 5.2
        /// </summary>
        /// <param name="col">the color to convert</param>
        /// <returns>an hsv representation of the specified color</returns>
        public static Vector3 RGBtoHSV(Color col)
        {
            float M = Mathf.Max(col.r, col.g, col.b);
            float m = Mathf.Min(col.r, col.g, col.b);
            float C = M - m;
            float V = M;
            float S = (C == 0) ? 0f : C / V;
            
            float H = 0f;
            if(M == col.r)
            {
                H = ((col.g - col.b) / C) % 6f;
            } else if (M == col.g)
            {
                H = (col.b - col.r) / C + 2f;
            } else
            {
                H = (col.r - col.g) / C + 4f;
            }
            H *= 60f;
            if (H < 0) H += 360f;
            return new Vector3(H, S, V);
        }

        /// <summary>
        /// Converts a hsv color value to an rgb color value. Used to relax necessity for Unity 5.3 to 5.2
        /// Credit to http://www.easyrgb.com/index.php?X=MATH&H=21#text21 for majority of concept
        /// </summary>
        /// <param name="H">hue [0, 360)</param>
        /// <param name="S">saturation [0, 1]</param>
        /// <param name="V">value [0, inf)</param>
        /// <param name="hdr">true for hdr values to be allowed</param>
        /// <returns>an rgb representation of the given hsv color</returns>
        public static Color HSVToRGB(float H, float S, float V, bool hdr)
        {
            if (S == 0f)
                return new Color(V, V, V);
            else if (V == 0f)
                return Color.black;
            else
            {
                Color col = Color.black;
                float Hval = H / 60f;
                int sel = Mathf.FloorToInt(Hval);
                float mod = Hval - sel;
                float v1 = V * (1f - S);
                float v2 = V * (1f - S * mod);
                float v3 = V * (1f - S * (1f - mod));
                switch (sel)
                {
                    
                    case 0:
                        col.r = V;
                        col.g = v3;
                        col.b = v1;
                        break;
                    case 1:
                        col.r = v2;
                        col.g = V;
                        col.b = v1;
                        break;
                    case 2:
                        col.r = v1;
                        col.g = V;
                        col.b = v3;
                        break;
                    case 3:
                        col.r = v1;
                        col.g = v2;
                        col.b = V;
                        break;
                    case 4:
                        col.r = v3;
                        col.g = v1;
                        col.b = V;
                        break;
                    default:
                        col.r = V;
                        col.g = v1;
                        col.b = v2;
                        break;
                }
                if(!hdr) {
                    col.r = Mathf.Clamp(col.r, 0f, 1f);
                    col.g = Mathf.Clamp(col.g, 0f, 1f);
                    col.b = Mathf.Clamp(col.b, 0f, 1f);
                }
                return col;
            }
        }

        /// <summary>
        /// Sets the baldness of the this instance of the character. True if the character is in denial.
        /// </summary>
        /// <param name="isBald">true => bald; false => has hair</param>
        public void SetBald(bool isBald)
        {
            hairRenderer.enabled = !isBald;
        }

        /// <summary>
        /// Sets the clothes of the Actor to the given material.
        /// </summary>
        /// <param name="material">The material of the new clothing</param>
        public void SetClothing(Texture texture)
        {
            clothes.mainTexture = texture;
        }
    }
}