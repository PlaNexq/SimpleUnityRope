using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRopeRenderer
{
    Transform[] RopeTransforms { get; set; }
    Material Material { get; set; }
    int Details { get; set; }
    float Radius { get; set; }

    public void Render();
}
