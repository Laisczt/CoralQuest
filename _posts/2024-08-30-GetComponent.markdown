---
layout: post
title:  "GetComponent()"
date:   2024-08-30 12:00:00 -0300
#categories: jekyll update
---
Quando precisamos que um script interaja com outros componentes em nossos objetos, vimos que é possível referenciá-los declarando uma variável pública e arrastando o componente no editor. Porém nem sempre é possível fazer isso; por exemplo, como você referenciaria o RigidBody2D de um objeto que foi instanciado depois de apertar play?

Para fazer isso, utilizamos a função GetComponent(). Dada uma referência ao GameObject do qual queremos pegar um componente, podemos usar:

{% highlight c# %}
RigidBody2D otherRigidBody;

void Start()
{
 otherRigidBody = otherGameObject.GetComponent<RigidBody2D>();
}
{% endhighlight %}

Existem várias formas de conseguir a referência ao GameObject, algumas são:

{% highlight c# %}
otherGameObject = GameObject.Find("nome objeto");
// encontra um objeto a partir do nome

childGameObject = GameObject.Find("nome pai/nome filho");
// encontra um objeto dentro de uma hierarquia

taggedObject = GameObject.FindWithTag("nome tag");
// encontra um objeto com uma determinada tag

firstChild = Parent.transform.GetChild(0).gameObject;
// Pega o primeiro filho de um objeto conhecido (travessia pela hierarquia geralmente é feita pelo Transform, não GameObject)
{% endhighlight%}

Como todos os objetos em Unity possuem GameObject e Transform, estes podem ser adquiridos usando ".transform" ou ".gameObject" a partir de qualquer componente
