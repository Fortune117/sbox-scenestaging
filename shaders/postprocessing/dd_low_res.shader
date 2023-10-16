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

	CreateTexture2D( tRenderTarget ) < Attribute( "render.target" );  	SrgbRead( true ); Filter( MIN_MAG_MIP_POINT ); AddressU( WRAP ); AddressV( WRAP ); >;

    float4 MainPs( PixelInput i ) : SV_Target0
    {
        float2 vScreenUv = i.vPositionSs.xy / g_vRenderTargetSize;

/*         float pixelation = 0.05f;
        float aspect = g_vRenderTargetSize.y / g_vRenderTargetSize.x;

        if ( pixelation > 0 )
        {
            float resolution = RemapValClamped( pow( pixelation, 0.1 ), 0, 1, g_vRenderTargetSize.x, 32 );
            float2 vPixelCount = float2( resolution, resolution * aspect );
            float2 pixelSize = 1.0f / vPixelCount;
            vScreenUv = floor(vScreenUv * vPixelCount) / vPixelCount;
            vScreenUv += pixelSize * 0.5f; // use center of pixel for sample
        } */

		float3 vColor = Tex2D(tRenderTarget, vScreenUv.xy).rgb;
        
        return float4( vColor, 1.0f );  
    }
}
