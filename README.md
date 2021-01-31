![logo](logo.png)

# HLHML
Un interpreteur pour des scripts écrits en français (High Level Humain Machine Language)

## Exemple de script
```
a vaut 5.
b vaut 7.

Afficher a " + " b " = ? ".

Lire la reponse.

Si la reponse est égal à a + b, afficher "Bonne réponse!" sinon afficher "Mauvaise réponse".
```

## Sortie
```
5 + 7 = ? 
```
L'utilisateur tape sa réponse
```
5 + 7 = ? 12
```
Le programme affiche
```
5 + 7 = ? 12
Bonne réponse!
```

## Lexique
<a href="https://github.com/freddycoder/HLHML/wiki/Lexique">Visiter la documentation</a>

## Générer l'image de l'arbre syntaxique
Selon le script suivant : 
```
i vaut 0.
Tant que i est plus petit que 3,
i = i + 1.
Si i est égal à 3, afficher i " ".
Ensuite, afficher i
```
En appelant l'executable en ligne de commande :
```
HLHML.Console.exe monScript.fr -t imageDestination.bmp
```
Le programme va produire l'image suvante
![impossible de trouver l'image...](https://raw.githubusercontent.com/freddycoder/HLHML/master/exempleAST.bmp)
</br>
Executer le même script en ligne de commande (projet HLHML.Console)
```
HLHML.Console.exe monScript.fr
```
La sortie sera
```
3 3
```
## HLHML.Editor

Le projet HLHML.Editor est une application WinForm qui permet de visualiser l'arbre syntaxique en même temps que d'écrire son script.

## HLHML.Syntaxe

Le repertoire HLHML.Syntaxe contient une extension vscode pour colorer les fichiers avec l'extension .fr

## Utiliser l'interpreteur dans une application tiers

1. Installer le package nuget.
2. Utiliser la classe Interpreteur pour interpreter du code.
```c#
using var sw = new StringWriter();
var interpreteur = new Interpreteur(sw);
interpreteur.Interprete("Afficher \"Bonjour le monde !\"");
Console.Out.WriteLine(sw.ToString());
// Bonjour le monde !
```

> Une instance de la classe Interpreteur garde la même 'Scope' pour les variables. Donc lancer deux script, ou deux commandes avec une même instance peut avoir des résultats différents selon le script.