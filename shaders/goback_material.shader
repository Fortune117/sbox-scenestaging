//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
    Description = "Go back - Material";
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
FEATURES
{
    #include "common/features.hlsl"
    Feature( F_FADE_BETWEEN, 0..1, "Psx" );
    Feature( F_FADE_TRANSITION, 0..1, "Psx" );
    FeatureRule( Requires1( F_FADE_TRANSITION, F_FADE_BETWEEN ), "Requires Fade Between" );

    Feature( F_TRANSLUCENT, 0..1, "Psx" );
    Feature( F_SELF_ILLUM, 0..1, "Psx" );
    Feature( F_BAKE_SELF_ILLUM, 0..1, "Psx" );
    FeatureRule( Requires1( F_BAKE_SELF_ILLUM, F_SELF_ILLUM ), "Requires Self Illum" );

    Feature( F_PBR, 0..1, "Psx" );
    Feature( F_TEST_VERTEX_SNAP, 0..1, "Psx" );
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
MODES
{
    VrForward();                                                    // Indicates this shader will be used for main rendering
    Depth(S_MODE_DEPTH);                                     // Shader that will be used for shadowing and depth prepass
    ToolsVis(S_MODE_TOOLS_VIS);                                     // Ability to see in the editor
    ToolsWireframe("vr_tools_wireframe.vfx");                     // Allows for mat_wireframe to work
    ToolsShadingComplexity("vr_tools_shading_complexity.vfx");     // Shows how expensive drawing is in debug view
}

//=========================================================================================================================
COMMON
{
    #include "common/shared.hlsl"

    float g_flAffineAmount < Range(0.0f, 1.0f); Default(0.2f); UiGroup("Go Back Settings,10/10"); > ;

    StaticCombo( S_TRANSLUCENT, F_TRANSLUCENT, Sys( ALL ) );
}

//=========================================================================================================================

struct VertexInput
{
    #include "common/vertexinput.hlsl"
};

//=========================================================================================================================

struct PixelInput
{
    #include "common/pixelinput.hlsl"
};

//=========================================================================================================================

VS
{
    #include "common/vertex.hlsl"

    StaticCombo( S_TEST_VERTEX_SNAP, F_TEST_VERTEX_SNAP, Sys( ALL ) );

    int g_RoundToDecimalPlace < Range(0, 10); Default(0); UiGroup("Go Back Settings,10/10"); > ;
    float g_flSnapScale < Range(1.0f, 5.0f); Default(2.0f); UiGroup("Go Back Settings,10/10"); > ;

    //
    // Main
    //
    PixelInput MainVs(INSTANCED_SHADER_PARAMS(VS_INPUT i))
    {
        PixelInput o = ProcessVertex(i);

        #if( !S_MODE_TOOLS_VIS || S_TEST_VERTEX_SNAP )
            // Apply a distance based fall-off to reduce Z-Fighting for distance objects but keep it for things up close!
            float flDistanceToPoint = RemapValClamped( distance( o.vPositionWs, g_vCameraPositionWs ), 500.0f, 5000.0f, 0.0f, 1.0f );
            // Vertex Snapping
            float flRound = pow(10, g_RoundToDecimalPlace) * g_flSnapScale;

            flRound = lerp(flRound, 10000.0f, flDistanceToPoint);

            o.vPositionPs.xyz = round(o.vPositionPs.xyz * flRound) / flRound;
        #endif
        // Affine texture mapping
        o.vTextureCoords *= lerp(1.0f, o.vPositionPs.w, g_flAffineAmount * 0.005f);

        return FinalizeVertex(o);
    }
}

//=========================================================================================================================

PS
{
    SamplerState CustomTextureFiltering < Filter(NEAREST); >;
    
    StaticCombo( S_MODE_DEPTH, 0..1, Sys( ALL ) );
    StaticCombo( S_FADE_BETWEEN, F_FADE_BETWEEN, Sys( ALL ) );
    StaticCombo( S_FADE_TRANSITION, F_FADE_TRANSITION, Sys( ALL ) );
    StaticCombo( S_PBR, F_PBR, Sys( ALL ) );
    StaticCombo( S_SELF_ILLUM, F_SELF_ILLUM, Sys( ALL ) );
    StaticCombo( S_BAKE_SELF_ILLUM, F_BAKE_SELF_ILLUM, Sys( ALL ) );

    StaticComboRule( Requires1( S_FADE_TRANSITION, S_FADE_BETWEEN ) );
    StaticComboRule( Requires1( S_BAKE_SELF_ILLUM, S_SELF_ILLUM ) );

    #include "common/pixel.hlsl"

    #if ( S_MODE_DEPTH )
        #define MainPs Disabled
    #endif

    #if ( S_SELF_ILLUM )
        CreateInputTexture2D( TextureSelfIllumMask, Srgb, 8, "", "_selfillum", "Material,10/80", Default3( 0.0, 0.0, 0.0 ) );
        CreateTexture2DWithoutSampler( g_tSelfIllumMask ) < Channel( RGB, Box( TextureSelfIllumMask ), Srgb ); OutputFormat( DXT1 ); SrgbRead( true ); >;

        #if( S_BAKE_SELF_ILLUM )
            TextureAttribute( LightSim_SelfIllumMaskTexture, g_tSelfIllumMask );
            
            Float3Attribute( LightSim_SelfIllumTint, g_vSelfIllumTint );
            FloatAttribute( LightSim_SelfIllumScale, g_flSelfIllumScale );
        #endif

        float3 g_vSelfIllumTint < UiType( Color ); Default3( 1.0, 1.0, 1.0 ); UiGroup( "Self Illum/20" ); >;
        float g_flSelfIllumBrightness < Default( 0.0 ); Range( -10.0, 10.0 ); UiGroup( "Self Illum" ); >;
        float3 g_vSelfIllumTintScaled < Expression( pow( 2.0, g_flSelfIllumBrightness ).xxx * saturate( g_flSelfIllumScale.xxx ) * SrgbGammaToLinear( g_vSelfIllumTint.xyz ) ); >;
    #endif


    #if( S_ALPHA_TEST || S_TRANSLUCENT )
        CreateTexture2DWithoutSampler( g_tTinkMask )  < Channel( R, Box( TextureTintMask ), Linear ); OutputFormat( BC7 ); SrgbRead( true ); >;
    #endif

    #if( S_FADE_BETWEEN )
        CreateInputTexture2D( TextureFadeColor,            Srgb,   8, "",                 "_color",  "Fade,20/10", Default3( 1.0, 1.0, 1.0 ) );
        CreateInputTexture2D( TextureFadeNormal,           Linear, 8, "NormalizeNormals", "_normal", "Fade,20/20", Default3( 0.5, 0.5, 1.0 ) );
        CreateInputTexture2D( TextureFadeRoughness,        Linear, 8, "",                 "_rough",  "Fade,20/30", Default( 0.5 ) );
        CreateInputTexture2D( TextureFadeMetalness,        Linear, 8, "",                 "_metal",  "Fade,20/40", Default( 1.0 ) );
        CreateInputTexture2D( TextureFadeAmbientOcclusion, Linear, 8, "",                 "_ao",     "Fade,20/50", Default( 1.0 ) );
        CreateInputTexture2D( TextureFadeBlendMask,        Linear, 8, "",                 "_blend",  "Fade,20/60", Default( 1.0 ) );
        CreateInputTexture2D( TextureFadeTintMask,     Linear, 8, "",                 "_tint",   "Fade,20/70", Default( 1.0 ) );
        #undef COLOR_TEXTURE_CHANNELS
        #if ( S_ALPHA_TEST || S_TRANSLUCENT )
            CreateInputTexture2D( TextureFadeTranslucency, Linear, 8, "",                 "_trans",  "Fade,20/70", Default3( 1.0, 1.0, 1.0 ) );
            #define COLOR_TEXTURE_CHANNELS Channel( RGB, AlphaWeighted( TextureFadeColor, TextureFadeTranslucency ), Srgb ); Channel( A, Box( TextureFadeTranslucency ), Linear )
        #else
            #define COLOR_TEXTURE_CHANNELS Channel( RGB,  Box( TextureFadeColor ), Srgb ); Channel( A, Box( TextureFadeTintMask ), Linear )
        #endif

        float3 g_flFadeTintColor < UiType( Color ); Default3( 1.0, 1.0, 1.0 ); UiGroup( "Fade,20/90" ); >;
        float g_flFadeAmount< Default( 0.0f ); Range(0.0f, 1.0f); UiGroup( "Fade,20,/100" ); >;

        CreateTexture2DWithoutSampler( g_tColorFade )  < COLOR_TEXTURE_CHANNELS; OutputFormat( BC7 ); SrgbRead( true ); >;
        CreateTexture2DWithoutSampler( g_tNormalFade ) < Channel( RGB, Box( TextureFadeNormal ), Linear ); OutputFormat( DXT5 ); SrgbRead( false ); >;

        #if( S_PBR )
            CreateTexture2DWithoutSampler( g_tRmaFade )    < Channel( R,    Box( TextureFadeRoughness ), Linear ); Channel( G, Box( TextureFadeMetalness ), Linear ); Channel( B, Box( TextureFadeAmbientOcclusion ), Linear );  Channel( A, Box( TextureFadeBlendMask ), Linear ); OutputFormat( BC7 ); SrgbRead( false ); >;
        #endif

        #if( S_ALPHA_TEST || S_TRANSLUCENT )
            CreateTexture2DWithoutSampler( g_tFadeTinkMask )  < Channel( R, Box( TextureFadeTintMask ), Linear ); OutputFormat( BC7 ); SrgbRead( true ); >;
        #endif

        #if( S_SELF_ILLUM )
            CreateInputTexture2D( TextureFadeSelfIllumMask, Srgb, 8, "", "_selfillum", "Fade,20/75", Default3( 0.0, 0.0, 0.0 ) );
            CreateTexture2DWithoutSampler( g_tFadeSelfIllumMask ) < Channel( RGB, Box( TextureFadeSelfIllumMask ), Srgb ); OutputFormat( DXT1 ); SrgbRead( true ); >;

            float g_flFadeSelfIllumScale < Default( 1.0 ); Range( 0.0, 16.0 ); UiGroup( "Self Illum" ); >;
            float3 g_vFadeSelfIllumTint < UiType( Color ); Default3( 1.0, 1.0, 1.0 ); UiGroup( "Self Illum/20" ); >;
            float g_flFadeSelfIllumBrightness < Default( 0.0 ); Range( -10.0, 10.0 ); UiGroup( "Self Illum" ); >;
            float3 g_vFadeSelfIllumTintScaled < Expression( pow( 2.0, g_flFadeSelfIllumBrightness ).xxx * saturate( g_flFadeSelfIllumScale.xxx ) * SrgbGammaToLinear( g_vFadeSelfIllumTint.xyz ) ); >;
        #endif
    #endif

    #if( S_FADE_TRANSITION )
        CreateInputTexture2D( TextureFadeTransition, Linear, 8, "",                 "_mask",     "Fade,20/80", Default( 1.0 ) );
        CreateTexture2DWithoutSampler( g_tFadeTransition )    < Channel( R,    Box( TextureFadeTransition ), Linear ); OutputFormat( BC7 ); SrgbRead( false ); >;
    #endif

    //
    // Main
    //
    float4 MainPs(PixelInput i) : SV_Target0
    {
        // Affine texture mapping
        i.vTextureCoords.xy /= lerp(1.0f, i.vPositionSs.w, g_flAffineAmount * 0.005f);

        float2 vUV = i.vTextureCoords.xy;

        float4 vRMA = float4( 1.0f, 0.0f, 1.0f, 1.0f );

        #if( S_PBR )
            vRMA = Tex2DS( g_tRma, CustomTextureFiltering, vUV );
        #endif

        Material m = Material::From( i,
                                    Tex2DS( g_tColor, CustomTextureFiltering, vUV  ), 
                                    Tex2DS( g_tNormal, CustomTextureFiltering, vUV  ), 
                                    vRMA, 
                                    g_flTintColor * i.vVertexColor.rgb );
        
        #if( S_ALPHA_TEST || S_TRANSLUCENT )
            m.Albedo.rgb = lerp( m.Albedo.rgb, m.Albedo.rgb * (g_flTintColor * i.vVertexColor.rgb), Tex2DS( g_tTinkMask, CustomTextureFiltering, vUV ).r );
        #endif

        #if( S_SELF_ILLUM )
            m.Emission = Tex2DS( g_tSelfIllumMask, CustomTextureFiltering, vUV ).rgb * g_vSelfIllumTintScaled;
        #endif

        #if( S_FADE_BETWEEN )
            #if( S_PBR )
                vRMA = Tex2DS( g_tRmaFade, CustomTextureFiltering, vUV );
            #endif

            Material mFade = Material::From( i,
                                            Tex2DS( g_tColorFade, CustomTextureFiltering, vUV  ), 
                                            Tex2DS( g_tNormalFade, CustomTextureFiltering, vUV  ), 
                                            vRMA, 
                                            g_flFadeTintColor * i.vVertexColor.rgb );

            #if( S_ALPHA_TEST || S_TRANSLUCENT )
                mFade.Albedo.rgb = lerp( mFade.Albedo.rgb, mFade.Albedo.rgb * (g_flFadeTintColor * i.vVertexColor.rgb), Tex2DS( g_tFadeTinkMask, CustomTextureFiltering, vUV ).r );
            #endif

            #if( S_SELF_ILLUM )
                mFade.Emission = Tex2DS( g_tFadeSelfIllumMask, CustomTextureFiltering, vUV ).rgb * g_vFadeSelfIllumTintScaled;
            #endif

            float flFadeAmount = g_flFadeAmount;

            #if( S_FADE_TRANSITION )
                float flDesiredFade = Tex2DS( g_tFadeTransition, CustomTextureFiltering, vUV ).r;
                if( g_flFadeAmount == 0.0f ) flFadeAmount = 0.0f;
                else if( flDesiredFade <= flFadeAmount ) flFadeAmount = 1.0f;
                else flFadeAmount = 0.0f;
                // flFadeAmount = Tex2DS( g_tFadeTransition, CustomTextureFiltering, vUV );
            #endif

            m = Material::lerp( m, mFade, flFadeAmount );
        #endif

        float4 vColor = ShadingModelStandard::Shade(i, m);

        #if ( S_MODE_TOOLS_VIS )
        {
            float3 vPositionWithOffsetWs = i.vPositionWithOffsetWs.xyz;
            float3 vPositionWs = i.vPositionWithOffsetWs.xyz + g_vHighPrecisionLightingOffsetWs.xyz;
            float3 vNormalWs = normalize(i.vNormalWs.xyz);

            ToolsVisInitColor(vColor.rgba);

            ToolsVisHandleFullbright(vColor.rgba, m.Albedo.rgb, vPositionWs.xyz, vNormalWs.xyz);

            ToolsVisHandleAlbedo(vColor.rgba, m.Albedo.rgb);
            ToolsVisHandleReflectivity(vColor.rgba, m.Albedo.rgb);
        }
        #endif

        return vColor;

    }
}