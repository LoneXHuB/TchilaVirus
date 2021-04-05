using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoneX.TchilaVirus
{
    public class BackgroundScroll : MonoBehaviour
    {
        public float length , height , startPosX , startPosY;
        public GameObject cam;
        public float parallaxEffect;

        private void Start()
        {
            startPosX = transform.position.x;
            startPosY = transform.position.y;
            this.transform.position = new Vector3(startPosX , startPosY , this.transform.position.z);
            length = transform.GetChild(1).position.x;
            height = transform.GetChild(6).position.y;
        }
        
        private void Update()
        {
            float tempX = (cam.transform.position.x *(1 - parallaxEffect));
            float tempY = (cam.transform.position.y *(1 - parallaxEffect));
            float distX = (cam.transform.position.x * parallaxEffect);
            float distY = (cam.transform.position.y * parallaxEffect);

            transform.position = new Vector3 (startPosX + distX , startPosY + distY , transform.position.z);
        
            if(tempX > startPosX + length ) startPosX += length;
            else if (tempX < startPosX - length ) startPosX -= length;

            if(tempY > startPosY + height ) startPosY += height;
            else if (tempY < startPosY - height ) startPosY -= height;
        }
        
    }   
}
