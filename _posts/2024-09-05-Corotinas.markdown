---
layout: post
title:  "Corotinas"
date:   2024-09-05 12:00:00 -0300
#categories: jekyll update
---

Uma das ferramentas mais importantes da Unity são as Corotinas, elas permitem dividir o fluxo de execução do script e fazer com que uma função seja executada ao longo de vários frames.

Corotinas são declaradas como funçãos com tipo IEnumerator e devem ser chamadas com a função StartCoroutine().

{% highlight c# %}

void Update()
{
	if(Input.GetKeyDown(KeyCode.Space))
	{
		StartCoroutine(DashForward(10));
	}
}

IEnumerator DashForward(int duracao)
{
/*
	Força o jogador a se mover rapidamente para a direita por alguns frames
*/
	mRigidBody = GetComponent<RigidBody2D>();
	
	while(duracao > 0)
	{	
		mRigidBody.velocity = Vector2.right * 5;
		duracao--;
		yield return null;
	}
}

{% endhighlight %}

Dentro de uma corotina, devemos usar a palavra ´yield´ para indicar onde a execução será pausada até o próximo frame. Podemos também utilizar outros valores no lugar de null, entre eles: ´yield return new WaitForFixedUpdate()´ retomará a execução no proximo frame de fixed update, 'yield return new WaitForSeconds(3)' retomará após 3 segundos.

