---
layout: post
title:  "Padrão Singleton"
date:   2024-09-03 12:00:00 -0300
#categories: jekyll update
---

Nem todos os objetos em Unity são criados da mesma forma, alguns tem maior importância. Você pode, por exemplo, ter um objeto GameManager, que possui funções como a de gameover, lida com o placar e mudanças de cena. Esse objeto será referenciado por várias classes pelo jogo, porém ele não pode ser estático, então referências à ele teriam que ser feitas da forma GameObject.Find("Manager") ou algo do tipo. Isso não é ideal, já que a função Find() não é eficiente, isso é o caso para a maioria das funções que atravéssam a hierarquia para encontrar um objeto, em graus diferentes.

O GameManager possui uma característica que pode nos ajudar a referênciá-lo de forma eficiente, isto é o fato que só pode existir um destes objetos ativos em um dado momento. Isso nos possibilita o uso do padrão Singleton de design.

{% highlight c# %}
public class GameManager : MonoBehaviour
{
	//
	//    ...
	//
	public int a = 3;
	public static GameManager Instance;
	
	void Awake()	// Awake ocorre ainda antes do Start()
	{
		Instance = this;
	}
	
	//
	//    ...
	//

}

{% endhighlight %}

Referências ao GameManager podem então ser feitas assim:

{% highlight c# %}
public class MinhaClasse : MonoBehaviour
{
	void Start()
	{
		Debug.Log(GameManager.Instance.a);
		// => 3
	}
}
{% endhighlight %}


[Mais sobre o padrão Singleton](https://gamedevbeginner.com/singletons-in-unity-the-right-way/)
