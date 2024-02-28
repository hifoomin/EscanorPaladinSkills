using System.Collections;

using TMPro;
using UnityEngine.UI;
using UnityEngine;
using RoR2.UI;
using RoR2;

namespace EscanorPaladinSkills.Components
{
    [DisallowMultipleComponent]
    public class TheOneHUD : MonoBehaviour
    {
        public HUD hud;
        public HGTextMeshProUGUI textMesh;
        public CharacterBody body;
        public TheOneController theOneController;

        public void Start()
        {
            hud = GetComponent<HUD>();

            var theOneContainer = new GameObject("TheOneContainer");
            // Main.logger.LogError("the one container is " + theOneContainer);

            var locator = hud.GetComponent<ChildLocator>();
            var upperRightCluster = locator.FindChild("TopCenterCluster").parent.Find("UpperRightCluster");
            // Main.logger.LogError("upper right cluster is " + upperRightCluster);
            theOneContainer.transform.SetParent(upperRightCluster);
            var rect = theOneContainer.AddComponent<RectTransform>();
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.localEulerAngles = Vector3.zero;
            var elem = theOneContainer.AddComponent<LayoutElement>();

            elem.minHeight = 120;
            elem.preferredWidth = 500;

            rect.pivot = new Vector2(1.15f, 0);
            rect.anchoredPosition = rect.pivot;

            //var image = uiContainer.AddComponent<Image>();
            //image.material = _hud.itemInventoryDisplay.GetComponentInChildren<Image>().material;

            //var textContainer = new GameObject("TextContainer");
            //textContainer.transform.SetParent(uiContainer.transform);
            //var textMesh = textContainer.AddComponent<HGTextMeshProUGUI>();
            textMesh = theOneContainer.AddComponent<HGTextMeshProUGUI>();
            textMesh.fontSize = 32;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.text = string.Empty;
        }

        public void TryInit()
        {
            if (!Run.instance)
            {
                return;
            }

            if (!hud.targetMaster)
            {
                return;
            }

            body = hud.targetMaster.GetBody();

            if (body && body.bodyIndex == Main.paladinBodyIndex)
            {
                theOneController = body.GetComponent<TheOneController>();
            }
        }

        public void Update()
        {
            if (!body || !theOneController)
            {
                textMesh.text = string.Empty;
                TryInit();
                return;
            }

            var transitionTimer = theOneController.transitionTimer;
            var theOneTimer = theOneController.theOneTimer;

            if (transitionTimer >= 60f * theOneController.timeMultiplier || theOneTimer <= 30f)
            {
                textMesh.color = new Color32(255, 105, 34, 255);
            }
            else if (transitionTimer <= 30f * theOneController.timeMultiplier || theOneTimer >= 50f)
            {
                textMesh.color = new Color32(204, 34, 34, 255);
            }
            else
            {
                textMesh.color = new Color32(204, 71, 34, 255);
            }

            var transIntegerPart = ((int)transitionTimer).ToString("#,0");
            var transDecimalPart = (transitionTimer - (int)transitionTimer).ToString("0.00").Substring(1);

            var integerPart = ((int)theOneTimer).ToString("#,0");
            var decimalPart = (theOneTimer - (int)theOneTimer).ToString("0.00").Substring(1);

            if (transitionTimer > 0f)
            {
                textMesh.text = "<mspace=0.5em>The One: " + transIntegerPart + "<sup>" + transDecimalPart + "</sup></mspace>";
            }
            else if (theOneTimer < -900f)
            {
                textMesh.text = string.Empty;
            }
            else
            {
                textMesh.text = "<mspace=0.5em>The One: " + integerPart + "<sup>" + decimalPart + "</sup></mspace>";
            }
        }
    }
}