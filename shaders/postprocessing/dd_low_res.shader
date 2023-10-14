HEADER
{
    Description = "Blink test";
    DevShader = true;
}

MODES
{
    Default();
    VrForward();
}

FEATURES
{
}

COMMON
{
    #include "postprocess/shared.hlsl"
}

struct VertexInput
{
    float3 vPositionOs : POSITION < Semantic( PosXyz ); >;
    float2 vTexCoord : TEXCOORD0 < Semantic( LowPrecisionUv ); >;
};

struct PixelInput
{
    float2 vTexCoord : TEXCOORD0;

	// VS only
	#if ( PROGRAM == VFX_PROGRAM_VS )
		float4 vPositionPs		: SV_Position;
	#endif

	// PS only
	#if ( ( PROGRAM == VFX_PROGRAM_PS ) )
		float4 vPositionSs		: SV_ScreenPosition;
	#endif
};

VS
{
    PixelInput MainVs( VertexInput i )
    {
        PixelInput o;
        o.vPositionPs = float4(i.vPositionOs.xyz, 1.0f); 
        o.vTexCoord = i.vTexCoord;
        return o;
    }
}

PS
{
    #include "postprocess/common.hlsl"

    RenderState( DepthWriteEnable, false );
    RenderState( DepthEnable, false );

    CreateTexture2D( g_tColorBuffer ) < Attribute( "ColorBuffer" );  	SrgbRead( true ); Filter( MIN_MAG_LINEAR_MIP_POINT ); AddressU( MIRROR ); AddressV( MIRROR ); >;
    CreateTexture2D( g_tDepthBuffer ) < Attribute( "DepthBuffer" ); 	SrgbRead( false ); Filter( MIN_MAG_MIP_POINT ); AddressU( CLAMP ); AddressV( CLAMP ); >;

	CreateTexture2D( tRenderTarget ) < Attribute( "render.target" );  	SrgbRead( true ); Filter( MIN_MAG_MIP_POINT ); AddressU( WRAP ); AddressV( WRAP ); >;

    float4 MainPs( PixelInput i ) : SV_Target0
    {
        // Get the current screen texture coordinates
        float2 vScreenUv = i.vPositionSs.xy / g_vRenderTargetSize;

		float3 vColor = Tex2D(tRenderTarget, vScreenUv.xy).rgb;
        
        // Invert the color and write it to our output
        return float4( vColor, 1.0f );  
    }
}
