using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    [Header("Preset")]
    [SerializeField] GameObject tentacleTip;
    [SerializeField] GameObject tentacleRoot;

    [Header("Bones")]
    [SerializeField] private int numBones = 10;
    [SerializeField] private float lengthTentacle = 50.0f;
    [SerializeField] private float lengthCapTentacle = 100.0f;

    [Header("Contraction")]
    [SerializeField] private float contractForceScaleRoot = 60.0f;
    [SerializeField] private float expandForceScaleRoot = 0.1f;
    [SerializeField] private float contractForceScaleTip = 100.0f;
    [SerializeField] private float expandForceScaleTip = 0.1f;

    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] private int additionSubBone = 0;

    // Start is called before the first frame update
    void Start()
    {
        int numSegments = numBones - 1;
        int totalBones = numSegments * additionSubBone + numBones;

        // Set linerenderer
        lineRenderer.positionCount = totalBones;

        //////////// Create tentacle joints //////////////
        List<GameObject> joints = new List<GameObject>();

        // Create root
        //tentacleRoot.transform.parent = transform;
        tentacleRoot.AddComponent<TentacleRoot>().setRenderer(0, lineRenderer);
        joints.Add(tentacleRoot);

        // Create joints
        for (int i=0;i<totalBones - 2; i++)
        {
            GameObject newJoint = new GameObject("TentacleJoint");
            newJoint.transform.parent = transform;
            newJoint.transform.position = transform.position;

            // Set renderer
            newJoint.AddComponent<TentacleJoint>().setRenderer(i + 1, lineRenderer);

            joints.Add(newJoint);
        }

        // Create tip
        //tentacleTip.transform.parent = transform;
        tentacleTip.AddComponent<TentacleTip>().setRenderer(totalBones - 1, lineRenderer);
        joints.Add(tentacleTip);

        // Connect main joints
        float mainJointLength = (lengthTentacle) / (numBones - 1);
        float mainJoinLengthCap = (lengthCapTentacle) / (numBones - 1);
        int mainIndexOffset = 1 + additionSubBone;
        for (int i = mainIndexOffset; i < joints.Count - 1; i += mainIndexOffset)
        {
            TentacleJoint tentacleJoint = joints[i].GetComponent<TentacleJoint>();

            // Set length
            tentacleJoint.initialize(joints[i - mainIndexOffset].transform, joints[i + mainIndexOffset].transform, mainJointLength, mainJoinLengthCap,
                contractForceScaleRoot, expandForceScaleRoot, contractForceScaleTip, expandForceScaleTip);
        }

        // Connect sub joints
        if(additionSubBone > 0)
        {
            float subJointLength = mainJointLength / (additionSubBone + 1);
            float subJointCapLength = mainJoinLengthCap / (additionSubBone + 1);
            for (int i = 1; i < joints.Count - 1; i += 1)
            {
                // Skip main joint
                if(i % (additionSubBone + 1) == 0)
                {
                    continue;
                }

                TentacleJoint tentacleJoint = joints[i].GetComponent<TentacleJoint>();

                // Set length
                tentacleJoint.initialize(joints[i - 1].transform, joints[i + 1].transform, subJointLength, subJointCapLength,
                    contractForceScaleRoot,expandForceScaleRoot,contractForceScaleTip,expandForceScaleTip);
            }
        }
    }
}
