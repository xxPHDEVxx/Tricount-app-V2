# Projet Tricount .NET

## Description

Le projet **Tricount .NET** est une application de gestion des dépenses entre utilisateurs, développée en C# avec le framework .NET. Conçu pour offrir une expérience plus esthétique et intuitive que sa version Java, Tricount .NET simplifie la répartition des coûts lors d'événements collectifs comme des voyages, des sorties entre amis ou des projets communs. Grâce à une interface moderne et élégante, cette application permet de créer, gérer et partager des comptes de dépenses (appelés "Tricounts") entre plusieurs participants.

<br>

<div>
    <img src="https://github.com/xxPHDEVxx/Tricount-app-V2/blob/master/presentation1.png" width="500" height="400">
    <img src="https://github.com/xxPHDEVxx/Tricount-app-V2/blob/master/presentation2.png" width="500" height="400">
</div>

## Fonctionnalités

- **Création et gestion des Tricounts** : Créez des Tricounts pour différents événements, ajoutez des participants et enregistrez les dépenses.
- **Répartition automatique des coûts** : Calculez automatiquement la part de chacun pour simplifier les remboursements.
- **Affichage des comptes** : Visualisez les comptes en cours, les détails des transactions, et les répartitions avec une interface claire et esthétique.
- **Interface utilisateur moderne** : Utilisation de WPF pour une expérience utilisateur riche et agréable.
- **Sauvegarde et récupération des données** : Conservez vos Tricounts et données d'utilisateurs pour une réutilisation future.

## Prérequis

- **.NET SDK** version 7.0
- **Rider** de JetBrains pour le développement C# (ou tout autre IDE compatible)
- **Windows** (pour l'exécution avec WPF, ou d'autres plateformes avec configurations spécifiques)

## Installation

1. Clonez le dépôt GitHub du projet :

    ```bash
    git clone https://github.com/xxPHDEVxx/votre-projet-tricount-dotnet.git
    cd votre-projet-tricount-dotnet
    ```

2. Ouvrez le projet dans Rider ou tout autre IDE compatible avec .NET.

3. Assurez-vous que toutes les dépendances sont bien installées et configurées.

4. Compilez et exécutez l'application à partir de l'IDE.

## Utilisation

1. **Connexion** : Connectez-vous avec votre compte pour accéder aux Tricounts existants ou en créer de nouveaux.

2. **Création d'un Tricount** : Après la connexion, créez un nouveau Tricount en ajoutant les détails de l'événement et les participants.

3. **Ajout des dépenses** : Ajoutez les dépenses au Tricount, en spécifiant le montant, le payeur, et les participants concernés. L'application calcule automatiquement la répartition des coûts.

4. **Visualisation des comptes** : Consultez l'état actuel des comptes, les montants dus, et les remboursements via une interface claire et agréable.

## Notes de version

### Version actuelle : 1.0.0

- **Ajout des fonctionnalités de base** : Création et gestion des Tricounts, répartition des dépenses, interface utilisateur améliorée.
- **Amélioration esthétique** : Utilisation de WPF pour une interface plus moderne et attrayante.

### Liste des bugs connus

- **Dispose manquant** : La fenêtre des opérations s'ouvre plusieurs fois après un logout/login, dû à un problème de gestion des ressources.

