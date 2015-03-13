var texte;
texte = GetComponentInChildren(MeshRenderer);
 function OnMouseEnter () {
 texte.material.color = Color.blue;
}
function OnMouseExit () {
 texte.material.color = Color.white;
}
function OnMouseUp () {
 Application.LoadLevel("Map1");
}

function OnMouseDown () {

 texte.material.color = Color.blue;
}