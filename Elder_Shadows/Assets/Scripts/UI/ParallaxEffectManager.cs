using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class ParallaxEffectManager : MonoBehaviour
{
        [SerializeField] private List<RectTransform> bg_layers_main;
        [SerializeField] private List<RectTransform> bg_layers_secondary;

        public float base_speed = 1;
        public float speed_increment = 0.5f;

        private void Update()
        {
                float counter1 = 0;
                foreach (var bg_layer in bg_layers_main)
                {
                        bg_layer.Translate(Vector3.left*(base_speed + counter1));
                        counter1 += speed_increment;
                        if (bg_layer.position.x <= -bg_layer.rect.width)
                        {
                                bg_layer.position = new Vector3(bg_layer.rect.width, bg_layer.position.y, 0f);
                        }
                }

                float counter2 = 0;
                foreach (var bg_layer in bg_layers_secondary)
                {
                        bg_layer.Translate(Vector3.left*(base_speed + counter2));
                        counter2 += speed_increment;
                        if (bg_layer.position.x <= -bg_layer.rect.width)
                        {
                                bg_layer.position = new Vector3(bg_layer.rect.width, bg_layer.position.y, 0f);
                        }
                }
        }
}
