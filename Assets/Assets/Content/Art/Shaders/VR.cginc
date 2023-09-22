#define PI 3.14159


inline float3 getLightRotation(float xAngle, float yAngle) {

	float3x3 yRot = float3x3(
		cos(yAngle),  0, sin(yAngle),
		0, 			  1, 		   0,
		-sin(yAngle), 0, cos(yAngle)
	);

	float3x3 xRot = float3x3(
		1, 0, 			0,
		0, cos(xAngle), -sin(xAngle),
		0, sin(xAngle), cos(xAngle)
	);

	return mul(yRot, mul(xRot, float3(0, 0, -1)));
}

