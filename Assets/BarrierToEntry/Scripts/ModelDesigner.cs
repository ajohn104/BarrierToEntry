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
                float hue = 0f;
                float sat = 0f;
                float val = 0f;
                Color.RGBToHSV(col, out hue, out sat, out val);
                beam.SetColor("_EmissionColor", Color.HSVToRGB(hue, 0.7720589f, 5f, true));
                beamCap.SetColor("_EmissionColor", Color.HSVToRGB(hue, 0.7720589f, 5f, true));
                light.color = col;
            }
            else {
                GetMat(part).color = col;
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