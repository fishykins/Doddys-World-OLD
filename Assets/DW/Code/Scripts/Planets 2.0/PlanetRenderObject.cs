using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw.planet {
	public class PlanetRenderObject {
        #region Variables
        public GameObject meshObject;
        #endregion;

        #region Properties

        #endregion;

        #region Custom Methods
        //Set this chunk to vissible... 
        public void SetVisible(bool visible = true)
        {
            if (meshObject != null) {
                meshObject.SetActive(visible);
            }
        }

        //Get is vissible
        public bool IsVissible()
        {
            if (meshObject != null) {
                return meshObject.activeSelf;
            }
            else {
                return false;
            }
        }
        #endregion;
    }
}