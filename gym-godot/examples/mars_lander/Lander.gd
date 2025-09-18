extends RigidBody3D

var jetStrength : float = 0.08
var lightEnergy : float = 0.15

func _ready() -> void:
	hide_all_flames()

func impulse(engine_id : String) -> void:
	hide_all_flames()
	if engine_id == "AuxX":
		$MeshFlameAuxX.visible = true
		$LightAuxX.light_energy = lightEnergy
		apply_impulse(-transform.basis.x*jetStrength, transform.basis * ($MeshFlameAuxX.transform.origin))
	elif engine_id == "AuxXn":
		$MeshFlameAuxXn.visible = true
		$LightAuxXn.light_energy = lightEnergy
		apply_impulse(transform.basis.x*jetStrength, transform.basis * ($MeshFlameAuxXn.transform.origin))
	elif engine_id == "AuxZ":
		$MeshFlameAuxZ.visible = true
		$LightAuxZ.light_energy = lightEnergy
		apply_impulse(-transform.basis.z*jetStrength, transform.basis * ($MeshFlameAuxZ.transform.origin))
	elif engine_id == "AuxZn":
		$MeshFlameAuxZn.visible = true
		$LightAuxZn.light_energy = lightEnergy
		apply_impulse(transform.basis.z*jetStrength, transform.basis * ($MeshFlameAuxZn.transform.origin))
	elif engine_id == "Main":
		$MeshFlameMain.visible = true
		$LightMain.light_energy = lightEnergy*2
		apply_impulse(4*transform.basis.y*jetStrength, transform.basis * ($MeshFlameMain.transform.origin))
	elif engine_id == "None":
		pass
		
func hide_all_flames() -> void :
	$MeshFlameAuxX.visible = false
	$MeshFlameAuxXn.visible = false
	$MeshFlameAuxZ.visible = false
	$MeshFlameAuxZn.visible = false
	$MeshFlameMain.visible = false
	$LightAuxX.light_energy = 0
	$LightAuxXn.light_energy = 0
	$LightAuxZ.light_energy = 0
	$LightAuxZn.light_energy = 0
	$LightMain.light_energy = 0
