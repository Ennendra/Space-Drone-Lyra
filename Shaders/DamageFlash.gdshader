shader_type canvas_item;

uniform vec4 flashColour : source_color = vec4(1.0);
uniform float flashModifier : hint_range(0.0,1.0) = 0;

void fragment() {
	// Called for every pixel the material is visible on.
	vec4 mainTexture = texture(TEXTURE,UV);
	mainTexture.rgb = mix(mainTexture.rgb, flashColour.rgb,flashModifier);
	COLOR = mainTexture;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
