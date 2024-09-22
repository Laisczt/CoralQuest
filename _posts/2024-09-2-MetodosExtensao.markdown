---
layout: post
title:  "Helper Classes & Extension Methods"
date:   2024-09-02 12:00:00 -0300
#categories: jekyll update
---

Às vezes, no nosso jogo, existem "funções" que precisamos utilizar em vários scripts diferentes. Ao invés de ter uma cópia dessa função em cada classe que precisa utilizá-la, podemos fazer uso de helper classes.

Uma helper Class é uma classe estática (pode ser chamada de qualquer parte do código) que guarda esse tipo de função que é útil em vários cenários.

O Código a seguir implementa uma helper class com uma função que eleva um número ao quadrado e o divide por 3

{% highlight c#%}
public static class Helpers // note que essa classe não herda de MonoBehaviour
{
	public static float SquareAndCut3(float x)
	{
		return x * x / 3;
	}
}
{% endhighlight %}

Essa função pode então ser utilizada em outras classes:

{% highlight c# %}
public class MinhaClasse : MonoBehaviour
{
	int a = 0;
	void Update()
	{
		Debug.Log(Helpers.SquareAndCut3(a));
		a++;
		// Imprime ao console o resultado da equação todo frame e incrementa o valor de entrada
	}
}
{% endhighlight %}

Uma ideia similar à de Helper Classes é Extension Classes.

Nós não temos acesso ao código das classes já presentes no Unity (ex.: Transform, AudioSource, Vector3), porém existem situações em que seria conveniente adicionar uma função à uma dessas classes.

Por exemplo, você pode achar que caso queira rotacionar um Vector2 em um ângulo específico, deve existir uma função Vector2.Rotate(angulo), porém essa função não existe.
Uma solução para isso seria implementar uma função RotateVector2() numa helper class, porém há uma solução mais elegante utilizando uma classe de extenção.:

{% highlight c# %}

public static class ExtensionMethods
{

	public static void Rotate(this Vector2 vec, float delta) 	// O termo 'this' no primeiro argumento define que isso é uma extensão da classe Vector2
	{
		vec =  new Vector2(
			vec.x * Mathf.Cos(delta) - vec.y * Mathf.Sin(delta),
			vec.x * Mathf.Sin(delta) + vec.y * Mathf.Cos(delta)
		);
	}


}

{% endhighlight %}

Podemos então usar a função da seguinte forma:

{% highlight c# %}
public class MinhaClasse : MonoBehaviour
{
	Vector2 v;
	float angulo = 3.14159f;
	
 	void Start()
 	{
 		v = new Vector2(1, 0);
 		v.Rotate(angulo);	// o primeiro argumento da função é o próprio vetor, não é necessário especificá-lo
 		Debug.Log(v);
 		// => (-1, 0)
 	}
}
{% endhighlight %}

