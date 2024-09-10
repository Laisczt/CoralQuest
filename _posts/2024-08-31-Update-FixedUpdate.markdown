---
layout: post
title:  "Update(), FixedUpdate() e deltaTime"
date:   2024-08-31 12:00:00 -0300
#categories: jekyll update
---

Vimos que a parte principal do código de objetos dinâmicos é feita dentro da função Update(), isto é, esse código rodará todo frame. Mas existe um problema com isso, um frame ocorre sempre que o Unity finaliza a execução de todo o código dentro de todos os Update()'s, o tempo entre frames pode variar à medida que o jogo cresce e, principalmente, a depender do hardware em que o jogo está rodando. Caso você desenvolva seu jogo em um computador que é capaz de rodar o jogo à 120 FPS (frames por segundo), um usuário com um computador que só é capaz de 60 FPS irá experienciar seu jogo em câmera lenta.

Para solucionar isso existem duas formas: uso de deltaTime e FixedUpdate().

## deltaTime

Time.deltaTime é uma variável do Unity que guarda o tempo em segundos que o computador demorou para calcular o último frame. Podemos utilizar esse valor para padronizar o movimento de objetos.

{% highlight c# %}
void Update()
{
	transform.position += Vector3.right * Time.deltaTime;
}
{% endhighlight %}

O Script acima move o objeto para a direita todo frame. Ao multiplicar a distância movida por deltaTime, garantimos que ela será proporcional a taxa de quadros e igual em qualquer computador

OBS: NEM SEMPRE O USO DE DELTATIME É TÃO SÍMPLES, E O EXEMPLO ACIMA SÓ SE APLICA À MOVIMENTO LINEAR (sem aceleração). [CLIQUE AQUI](https://www.youtube.com/watch?v=yGhfUcPjXuE&t=205s&pp=ygUJZGVsdGFUaW1l) PARA MAIS DETALHES SOBRE USO DE DELTATIME

Geralmente, utilizar deltaTime é a forma mais versátil de garantir consistência no comportamento de seu jogo, porém pode ser complicado em algumas situações.

## FixedUpdate()

FixedUpdate(), por outro lado, ocorre exatamente 60 vezes por segundo(configurável), por isso sempre é consistente entre máquinas*. Apesar disso, Time.fixedDeltaTime está disponível caso necessário.

\* Contanto que o computador seja capaz de calcular 60 quadros por segundo. Se esse não for o caso, Update() deve ser utilizado

FixedUpdate() também tem outra característica importante: todo o cálculo de física em Unity (colisões, RigidBody, etc.) é feito nele, então é uma boa ideia colocar partes do código que interajam com a física dentro de FixedUpdate().

Dado isso, existe uma parte do código que é recomendada sempre estar em Update(): Input do usuário.

Um exemplo de movimento horizontal usando Update() e FixedUpdate():

{% highlight c# %}

RigidBody2D myRB;
Float InputX;

void Start()
{
	myRB = GetComponent<RigidBody2D>();
}

void Update()
{
	InputX = Input.GetAxis("Horizontal");
	// Esse valor varia entre -1 (esquerda), 0 (parado) e 1(direita)
}

void FixedUpdate()
{
	myRB.velocity = new Vector2(InputX, 0);
}

{% endhighlight %}

