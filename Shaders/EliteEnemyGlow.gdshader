shader_type canvas_item;

uniform vec4 glowColour : source_color = vec4(0,1.0,1.0,0.8);

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec4 mainTexture = texture(TEXTURE,UV);
	mainTexture.rgb = mix(mainTexture.rgb, glowColour.rgb,1.0);
	COLOR = mainTexture;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
