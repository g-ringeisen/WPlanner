# WurthPlanner.MongoDbInitializer

Console .NET 10 d'initialisation idempotente de MongoDB pour Würth Planner.

## Positionnement

Placer ce dossier à la racine du dépôt, au même niveau que le dossier `WurthPlanner`. Le `ProjectReference` utilise alors les modèles et l'implémentation MongoDB existants du dépôt, notamment `MongoDbInitializer` et `ServiceExtensions`.

## Ce qui est initialisé

Le programme crée de manière idempotente les collections MongoDB dès la création de leurs index et configure les index nécessaires aux requêtes applicatives :

- `workItems` : `ParentId`, `DueDate`, `(Status, Type)`, `UpdatedAt` décroissant ;
- `assignments` : `WorkItemId`, `(AssigneeId, StartDate, EndDate)` ;
- `timeEntries` : `(WorkItemId, WorkDate)`, `AssignmentId`, `(EmployeeId, WorkDate)` ;
- `notes` : `(WorkItemId, CreatedAt)`.

Il ne crée aucune donnée métier de démonstration et ne supprime ni ne modifie de données existantes.

## Configuration

### Fichier local

Mettre à jour `appsettings.json` pour un environnement de développement :

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "wurth-planner"
  }
}
```

### Variables d'environnement

Pour éviter d'exposer la chaîne de connexion, les variables préfixées par `WPLANNER_` sont prises en charge :

```bash
export WPLANNER_MongoDb__ConnectionString='mongodb://utilisateur:mot-de-passe@serveur:27017/?authSource=admin'
export WPLANNER_MongoDb__DatabaseName='wurth-planner'
```

### Ligne de commande

Les mêmes clés peuvent être passées au lancement :

```bash
dotnet run --project WurthPlanner.MongoDbInitializer -- \
  --MongoDb:ConnectionString 'mongodb://localhost:27017' \
  --MongoDb:DatabaseName 'wurth-planner'
```

## Exécution

```bash
dotnet restore
dotnet run --project WurthPlanner.MongoDbInitializer
```

L'opération est idempotente : elle peut être exécutée à chaque déploiement. MongoDB conserve les index qui existent déjà.

## Intégration dans la solution

Depuis la racine du dépôt :

```bash
dotnet sln WurthPlanner.slnx add WurthPlanner.MongoDbInitializer/WurthPlanner.MongoDbInitializer.csproj
```
