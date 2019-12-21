using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class Tarjetas : MonoBehaviour {
	
	private GameObject[] tarjetas;
	private string[] figuras;
	private Image fondo_tarjeta;
	private Sprite fondo_default, fondo_figura;
	private Button boton;
	private Text texto_tarjeta;
	private bool[] tarjeta_volteada;
	private Dictionary<string, int> tarjetas_par;
	private List <string> claves;
	private bool? par;
	private int intento;

	void Start () {

		par = null;
		intento = 0;

		//Arreglo que indica el numero de la tarjeta volteada
		//La posicion 'i' indica la tarjeta
		//El valor 'true' indica que la tarjeta 'i' fue volteada
		tarjeta_volteada = new bool[12];
		for(int i = 0; i < tarjeta_volteada.Length; i++) {
			tarjeta_volteada[i] = false;
		}

		//Arreglo con las figuras que aparecen en las tarjetas
		//La figura en la posicion 'i' aparece en la tarjeta 'i'
		figuras = new string[] {"Ave", "Cangrejo", "Gato", "Oso", "Pato", "Pinguino", 
								"Ave", "Cangrejo", "Gato", "Oso", "Pato", "Pinguino"};
		//Genera un nuevo orden en el arreglo de figuras
		shuffle ();

		//Arreglo que marca cuantas tarjetas con una figura se han volteado
		//El valor maximo que puede tener cada figura es dos
		tarjetas_par = new Dictionary<string, int> (){
			{"Ave", 0}, {"Cangrejo", 0}, {"Gato", 0}, {"Oso", 0}, {"Pato", 0}, {"Pinguino", 0}
		};
		claves = new List<string> (tarjetas_par.Keys);

		//Genera arreglo con los objetos 'tarjetas'
		tarjetas = GameObject.FindGameObjectsWithTag ("BTarjeta");

		//Respalda la imagen de fondo de las tarjetas
		fondo_default = tarjetas [0].GetComponent<Image> ().sprite;

		//Configura las tarjetas como botones para que ejecuten la funcion TaskOnClick cuando sean presionadas
		for(int i = 0;  i < tarjetas.Length; i++){
			
			boton = tarjetas[i].GetComponent<Button> ();
			int _index = i;
			boton.onClick.AddListener ( () => {TaskOnClick(_index);} );

		}
	}

	//Recibe el numero de tarjeta que se voltea
	private void TaskOnClick(int index){

		fondo_tarjeta = tarjetas[index].GetComponent<Image>();
		texto_tarjeta = tarjetas[index].GetComponentInChildren<Text> ();

		//Condicion que bloquea las tarjetas que ya han sido emparejadas
		if (tarjetas_par [figuras [index]] != 2) {
			if (tarjeta_volteada [index] == true) {
				//Voltea la tarjeta ocultando su figura
				intento--;
				//Validacion de variable
				if (intento < 0) {
					intento = 0;
				}
				tarjeta_volteada [index] = false;
				volteaTarjeta (fondo_default, "Memorama");
				cuentaTarjetaVolteada (figuras [index], false);

			} else {
				//Muestra la figura de la tarjeta
				intento++;
				//Validacion de variable
				if (intento > 2) {
					intento = 2;
				}
				tarjeta_volteada [index] = true;
				fondo_figura = Resources.Load<Sprite> (figuras [index]);
				volteaTarjeta (fondo_figura, "");
				cuentaTarjetaVolteada (figuras [index], true);
			}
		}
	}

	private void cuentaTarjetaVolteada(string key, bool accion){

		//Aumenta o disminuye el numero de tarjetas con la figura 'key' volteadas
		if (accion == true) {
			tarjetas_par [key]++ ;
			//Validacion de variable
			if(tarjetas_par [key] > 2){
				tarjetas_par [key] = 2;
			}
		} else {
			tarjetas_par [key]-- ;
			//Validacion de variable
			if(tarjetas_par [key] < 0){
				tarjetas_par [key] = 0;
			}
		}

		//Si se han volteado dos tarjetas verificar si son de la misma figura
		if (intento == 2) {
			
			if (tarjetas_par [key] == 2) {
				//Si las tarjetas son de la misma figura...
				par = true;
				intento = 0;
			} else {
				//Si las tarjetas son de distinta figura entonces
				//oculta la figura de la tarjeta volteada anteriormente
				//y solo muestra la figura de la ultima tarjeta volteada
				par = false;
				intento = 1;

				//Reinicia contador de tarjetas volteadas que no se han emparejado
				foreach (string clave in claves) {
					if (tarjetas_par [clave] == 1 && clave.Equals(key) == false) {
						tarjetas_par [clave] = 0;
					}
				}

				//Voltea todas las tarjetas que no se han emparejado
				for(int i = 0; i < tarjetas.Length; i++){
					if(tarjeta_volteada[i] == true){
						if(figuras[i].Equals(key) == false && tarjetas_par[figuras[i]] != 2){
							fondo_tarjeta = tarjetas[i].GetComponent<Image>();
							texto_tarjeta = tarjetas[i].GetComponentInChildren<Text> ();
							volteaTarjeta (fondo_default, "Memorama");
							tarjeta_volteada [i] = false;
						}
					}
				}

			}
		}
	}

	//Cambia la apariencia de la tarjeta simulando que se voltea una tarjeta
	private void volteaTarjeta(Sprite fondo, string textoT) {
		fondo_tarjeta.sprite = fondo;
		texto_tarjeta.text = textoT;
	}

	//Genera un nuevo orden en el arreglo de figuras
	//Ademas actualiza el arreglo de relacion entre figura y tarjeta
	private void shuffle() {
		
		for(int i = 0; i < figuras.Length; i++){
			string temp = figuras [i];
			int randomIndex = Random.Range (0, figuras.Length);
			figuras [i] = figuras [randomIndex];
			figuras [randomIndex] = temp;
		}
	}

	//Indica si se ha formado un nuevo par despues de voltear dos tarjetas
	//El valor solo puede ser consultado una vez
	//Regresa null cuando se ha volteado solo una tarjeta
	public bool? nuevoPar(){
		bool? aux = par;
		par = null;
	
		return aux;
	}

	//Verifica si se han volteado todas las tarjetas
	public bool finJuego(){
		foreach(KeyValuePair <string, int> par in tarjetas_par){
			if( par.Value != 2){
				return false;
			}
		}

		return true;
	}

	//Reinicia juego
	public void reiniciarJuego(){

		par = null;
		intento = 0;

		//Reordena figuras en tarjetas
		shuffle ();

		//Reinicia contador de tarjetas volteadas
		foreach (string clave in claves) {
			tarjetas_par [clave] = 0;
		}

		//Voltea todas las tarjetas
		for(int i = 0; i < tarjetas.Length; i++){
			
			fondo_tarjeta = tarjetas[i].GetComponent<Image>();
			texto_tarjeta = tarjetas[i].GetComponentInChildren<Text> ();

			volteaTarjeta (fondo_default, "Memorama");
			tarjeta_volteada [i] = false;
		}

	}

}
