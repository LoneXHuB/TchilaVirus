using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.U2D;
using System.Reflection;

namespace LoneX.TchilaVirus
{
    public class SpriteshapeShadowSnapper : MonoBehaviour
    {
        #region PublicAtt
        public SpriteShapeController spriteShape;
        public bool isDynamicSnap;
        #endregion

        #region PrivateAtt
        ShadowCaster2D shadowCaster;
        #endregion

        #region MonoBehaviourCallbacks
            // Start is called before the first frame update
            void Start()
            {
                shadowCaster = this.GetComponent<ShadowCaster2D>();
                SnapShadowCaster(); 
            }
            // Update is called once per frame
            void Update()
            {  
               if(isDynamicSnap)
                    SnapShadowCaster(); 
            }
        #endregion

        #region 5-3-8 methods ;) 
                ///<summary>
            /// Replaces the path that defines the shape of the shadow caster.
            /// </summary>
            /// <remarks>
            /// Calling this method will change the shape but not the mesh of the shadow caster. Call SetPathHash afterwards.
            /// </remarks>
            /// <param name="shadowCaster">The object to modify.</param>
            /// <param name="path">The new path to define the shape of the shadow caster.</param>
            public static void SetPath(ShadowCaster2D shadowCaster, Vector3[] path)
            {
                FieldInfo shapeField = typeof(ShadowCaster2D).GetField("m_ShapePath",
                                                                    BindingFlags.NonPublic |
                                                                    BindingFlags.Instance);
                shapeField.SetValue(shadowCaster, path);
            }
        
            /// <summary>
            /// Replaces the hash key of the shadow caster, which produces an internal data rebuild.
            /// </summary>
            /// <remarks>
            /// A change in the shape of the shadow caster will not block the light, it has to be rebuilt using this function.
            /// </remarks>
            /// <param name="shadowCaster">The object to modify.</param>
            /// <param name="hash">The new hash key to store. It must be different from the previous key to produce the rebuild. You can use a random number.</param>
            public static void SetPathHash(ShadowCaster2D shadowCaster, int hash)
            {
                FieldInfo hashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash",
                                                                    BindingFlags.NonPublic |
                                                                    BindingFlags.Instance);
                hashField.SetValue(shadowCaster, hash);
            }
        #endregion
        
        #region OtherCallbacks
        #endregion

        #region PublicMeths
        #endregion

        #region PrivateMeths
        private void SnapShadowCaster()
        {
            Vector3[] _points = new Vector3[spriteShape.spline.GetPointCount()];
                for(int i = 0 ; i < spriteShape.spline.GetPointCount() ; i++)
                {
                    _points[i] = spriteShape.spline.GetPosition(i);
                }
                SetPath(shadowCaster , _points);
                SetPathHash(shadowCaster , Random.Range(int.MinValue, int.MaxValue)); // The hashing function GetShapePathHash could be copied from the LightUtility class
            
        }
        #endregion
        
    }
}
