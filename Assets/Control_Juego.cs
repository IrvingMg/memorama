using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Control_Juego : MonoBehaviour {

	//private IEnumerator jugar;
	private GameObject[] botones;
	private Text marcador;
	private Button btn;
	private Tarjetas conjuntoTarjetas;
	private bool jugar;
	private AudioSource audioS;
	private AudioClip audioClic, audioPar, audioNoPar, audioFinJuego;
	private bool? banderaPar;
	private int puntos;
	private GridLayoutGroup grid;

	// Use this for initialization
	void Start () {

		//Inicializa el funcionamiento de los botones
		botones = GameObject.FindGameObjectsWithTag ("Boton");

		foreach (GameObject obj in botones) {
			btn = obj.GetComponent<Button> ();
			asignaAccion(btn.name);
		}

		//Carga el sonido de los botones
		audioS = GetComponent<AudioSource> ();
		audioClic = Resources.Load<AudioClip>("Sonidos/timer_click");

		//Si la escena activa es la 2 entonces carga los elementos necesarios
		if (SceneManager.GetActiveScene ().name.CompareTo ("Escena2") == 0) {
			jugar = true;

			grid = GameObject.Find ("Contenedor").GetComponent<GridLayoutGroup> ();
			detectarOrientacion ();
			conjuntoTarjetas = GetComponent<Tarjetas> ();
			marcador = GameObject.Find ("Marcador").GetComponent<Text> (); 
			audioPar =  Resources.Load<AudioClip>("Sonidos/right_answer");
			audioNoPar =  Resources.Load<AudioClip>("Sonidos/wrong_answer");
			audioFinJuego =  Resources.Load<AudioClip>("Sonidos/lesson_complete");
			puntos = 0;
			marcador.text = puntos + " Puntos";

		} else {
			jugar = false;
		}


	}
	
	// Update is called once per frame

	void Update () {
		detectarOrientacion ();

		if (jugar == true) {
			
			banderaPar = conjuntoTarjetas.nuevoPar ();

			if ( banderaPar == true) {
				reproduceSonido (audioPar);
				incrementaPuntos (true);


				if (conjuntoTarjetas.finJuego () == true) {
					reproduceSonido (audioFinJuego);
				}

			} else {
				if (banderaPar == false) {
					reproduceSonido (audioNoPar);
					incrementaPuntos (false);
				}
			}

		}
	}

	private void detectarOrientacion(){
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft) {
			grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
		}
		else if (Input.deviceOrientation == DeviceOrientation.Portrait) {
			grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		}
	}

	private void incrementaPuntos(bool bandera){
		if (bandera == true) {
			puntos += 50;
		} else {
			puntos -= 5;
			if (puntos < 0) {
				puntos = 0;
			}
		}

		marcador.text = puntos + " Puntos";
	}

	private void reproduceSonido(AudioClip sonido){
		audioS.clip = sonido;
		audioS.Play ();
	}

	private void asignaAccion(string name){

		if( name.Equals("Iniciar") == true){
			btn.onClick.AddListener (botonIniciar);
		}

		if (name.Equals ("Salir") == true) {
			btn.onClick.AddListener (botonSalir);
		}

		if( name.Equals("Home") == true){
			btn.onClick.AddListener (botonMenu);
		}
		if( name.Equals("Reiniciar") == true){
			btn.onClick.AddListener (botonReiniciar);
		}

	}

	public void botonSalir(){
		reproduceSonido (audioClic);
		Application.Quit ();
	}

	public void botonIniciar(){
		reproduceSonido (audioClic);
		SceneManager.LoadScene ("Escena2");
	}

	public void botonMenu(){
		reproduceSonido (audioClic);
		SceneManager.LoadScene ("Escena1");
	}

	public void botonReiniciar(){
		reproduceSonido (audioClic);
		conjuntoTarjetas.reiniciarJuego();
		puntos = 0;
		marcador.text = puntos + " Puntos";
	}

}

