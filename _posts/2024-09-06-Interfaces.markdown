---
layout: post
title:  "Interfaces"
date:   2024-09-06 12:00:00 -0300
#categories: jekyll update
---

Uma interface, nesse contexto, não se trata da interface de usuário(UI), mas de uma ferramenta de Orientação à Objetos que nos permite abstrair classes à um conjunto de métodos comuns a todas elas.

Pegue por exemplo, Um ataque do jogador que pode atingir inimigos e objetos quebráveis, podemos implementá-lo da seguinte forma:

{% highlight c# %}
public interface IDamageable
{
	void Damage(int amount);
}
{% endhighlight %}

No script do ataque

{% highlight c# %}
void OnTriggerEnter2D(Collider2D collision)
{
	hit = collision.GetComponent<IDamageable>()
	if(hit != null)
	{
		hit.Damage(dano); 
	}
}
{% endhighlight %}

No script do inimigo
{% highlight c# %}
public class Enemy: MonoBehaviour, IDamageable
{
	int health = 100;
	/* Implementação da classe
	.
	.
	.
	*/
	
	
	void Damage(int amount)
	{
		health -= amount;
	}
}
{% endhighlight %}

No script do objeto quebrável
{% highlight c# %}
public class Crate: MonoBehaviour, IDamageable
{
	/* Implementação da classe
	.
	.
	.
	*/
	
	void BreakObject()
	{
		Destroy(gameObject);
	}
	
	void Damage(int amount)
	{
		BreakObject();
	}
}

{% endhighlight %}
