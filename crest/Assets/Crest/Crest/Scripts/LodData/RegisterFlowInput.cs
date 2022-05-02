﻿// Crest Ocean System

// This file is subject to the MIT License as seen in the root of this folder structure (LICENSE)

using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Crest
{
    /// <summary>
    /// Registers a custom input to the flow data. Attach this GameObjects that you want to influence the horizontal flow of the water volume.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu(MENU_PREFIX + "Flow Input")]
    [HelpURL(Internal.Constants.HELP_URL_BASE_USER + "ocean-simulation.html" + Internal.Constants.HELP_URL_RP + "#flow")]
    public class RegisterFlowInput : RegisterLodDataInputWithSplineSupport<LodDataMgrFlow, SplinePointDataFlow>, IPaintedDataClient
    {
        /// <summary>
        /// The version of this asset. Can be used to migrate across versions. This value should
        /// only be changed when the editor upgrades the version.
        /// </summary>
        [SerializeField, HideInInspector]
#pragma warning disable 414
        int _version = 0;
#pragma warning restore 414

        public override bool Enabled => true;

        public override float Wavelength => 0f;

        protected override Color GizmoColor => new Color(0f, 0f, 1f, 0.5f);

        protected override string ShaderPrefix => "Crest/Inputs/Flow";

        protected override bool FollowHorizontalMotion => _followHorizontalMotion;

        protected override string SplineShaderName => "Hidden/Crest/Inputs/Flow/Spline Geometry";
        protected override Vector2 DefaultCustomData => new Vector2(SplinePointDataFlow.k_defaultSpeed, 0f);

        #region Painting
        public CPUTexture2DPaintable_RG16_AddBlend _paintedInput;
        protected override void PreparePaintInputMaterial(Material mat)
        {
            base.PreparePaintInputMaterial(mat);

            _paintedInput.CenterPosition3 = transform.position;
            _paintedInput.GraphicsFormat = GraphicsFormat;
            _paintedInput.PrepareMaterial(mat, CPUTexture2DHelpers.ColorConstructFnTwoChannel);
        }
        protected override void UpdatePaintInputMaterial(Material mat)
        {
            base.UpdatePaintInputMaterial(mat);

            _paintedInput.CenterPosition3 = transform.position;
            _paintedInput.GraphicsFormat = GraphicsFormat;
            _paintedInput.UpdateMaterial(mat, CPUTexture2DHelpers.ColorConstructFnTwoChannel);
        }
        protected override Shader PaintedInputShader => Shader.Find("Hidden/Crest/Inputs/Flow/Painted");
        public GraphicsFormat GraphicsFormat => GraphicsFormat.R16G16_SFloat;

        public CPUTexture2DBase Texture => _paintedInput;
        public Vector2 WorldSize => _paintedInput.WorldSize;
        public float PaintRadius => _paintedInput._brushRadius;
        public Transform Transform => transform;

        public void ClearData()
        {
            _paintedInput.Clear(this, Vector2.zero);
        }

        public bool Paint(Vector3 paintPosition3, Vector2 paintDir, float paintWeight, bool remove)
        {
            _paintedInput.CenterPosition3 = transform.position;

            return _paintedInput.PaintSmoothstep(this, paintPosition3, 0.25f * paintWeight, paintDir, CPUTexture2DHelpers.PaintFnAdditivePlusRemoveBlendVector2, remove);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_paintedInput == null)
            {
                _paintedInput = new CPUTexture2DPaintable_RG16_AddBlend();
            }

            _paintedInput.Initialise(this);
        }
        #endregion

        [Header("Other Settings")]

        [SerializeField, Tooltip(k_displacementCorrectionTooltip)]
        bool _followHorizontalMotion = false;

#if UNITY_EDITOR
        protected override string FeatureToggleName => "_createFlowSim";
        protected override string FeatureToggleLabel => "Create Flow Sim";
        protected override bool FeatureEnabled(OceanRenderer ocean) => ocean.CreateFlowSim;

        protected override string RequiredShaderKeywordProperty => LodDataMgrFlow.MATERIAL_KEYWORD_PROPERTY;
        protected override string RequiredShaderKeyword => LodDataMgrFlow.MATERIAL_KEYWORD;

        protected override string MaterialFeatureDisabledError => LodDataMgrFlow.ERROR_MATERIAL_KEYWORD_MISSING;
        protected override string MaterialFeatureDisabledFix => LodDataMgrFlow.ERROR_MATERIAL_KEYWORD_MISSING_FIX;
#endif // UNITY_EDITOR
    }
}
