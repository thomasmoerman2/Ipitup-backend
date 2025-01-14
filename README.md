**Git Branch Workflow Richtlijnen**

Om een georganiseerde en efficiënte samenwerking binnen ons team te garanderen, hanteren we de volgende workflow voor het werken met branches in Git.

---

### **1. Main Branch**

- **Doel**: De `main` branch is uitsluitend bedoeld voor de productieversie van ons project.
- **Regels**:
  - Geen directe commits of merges naar de `main` branch.
  - Alleen geteste en goedgekeurde wijzigingen uit de `release` branch mogen hier worden gemerged.

### **2. Release Branch**

- **Doel**: De `release` branch wordt gebruikt om de productversie te testen voordat deze naar de `main` branch gaat.
- **Regels**:
  - Alleen stabiele en geteste wijzigingen uit de `develop` branch worden hier naartoe gemerged.
  - Testen van de productversie gebeurt op deze branch.

### **3. Develop Branch**

- **Doel**: De `develop` branch is de centrale branch waar de actieve ontwikkeling plaatsvindt.
- **Regels**:
  - Alle nieuwe functies en fixes worden via feature- of fix-branches naar de `develop` branch gemerged.

### **4. Feature Branches**

- **Doel**: Nieuwe functionaliteiten worden ontwikkeld in aparte feature-branches.
- **Naamconventie**: `feature/{naam-van-feature}`
  - Bijvoorbeeld: `feature/homepage`
- **Regels**:
  - Maak een nieuwe branch vanuit de `develop` branch.
  - Na voltooiing en testen wordt de branch terug naar `develop` gemerged.

### **5. Fix Branches**

#### **5.1 Bugfix Branch**

- **Doel**: Kleine bugfixes worden afgehandeld in aparte fix-branches.
- **Naamconventie**: `fix/{naam-van-fix}`
  - Bijvoorbeeld: `fix/header-logo`
- **Regels**:
  - Maak een nieuwe branch vanuit de branch waarin het probleem zich bevindt.
  - Na voltooiing en testen wordt de branch terug naar de branch waarin het probleem zich bevindt gemerged.

#### **5.2 Hotfix Branch**

- **Doel**: Kritieke bugs die de werking van het programma ernstig verstoren worden afgehandeld in hotfix-branches.
- **Naamconventie**: `hotfix/{naam-van-hotfix}`
  - Bijvoorbeeld: `hotfix/crash-on-start`
- **Regels**:
  - Maak een nieuwe branch vanuit de branch waarin het probleem zich bevindt.
  - Na voltooiing en testen wordt de branch terug naar de branch waarin het probleem zich bevindt gemerged.

---

### **Overzicht Workflow**

1. **Nieuwe functies**:

   - Begin een nieuwe branch vanuit `develop` met de naam `feature/{naam-van-feature}`.
   - Ontwikkel en test de functie.
   - Merge de branch terug naar `develop`.

2. **Kleine bugs**:

   - Begin een nieuwe branch vanuit de branch waarin het probleem zich bevindt met de naam `fix/{naam-van-fix}`.
   - Los het probleem op en test.
   - Merge de branch terug naar de branch waarin het probleem zich bevindt.

3. **Kritieke bugs**:

   - Begin een nieuwe branch vanuit branch waarin het probleem zich bevindt met de naam `hotfix/{naam-van-hotfix}`.
   - Los het probleem op en test.
   - Merge de branch naar de branch waarin het probleem zich bevindt.

4. **Release voorbereiding**:

   - Merge stabiele wijzigingen van `develop` naar `release`.
   - Test de wijzigingen in de `release` branch.
   - Als de tests succesvol zijn, merge de wijzigingen van `release` naar `main`.

---

### **Belangrijk**

- Gebruik duidelijke en beschrijvende namen voor branches.
- Zorg ervoor dat alle wijzigingen getest zijn voordat ze worden gemerged.
- Respecteer de branch-hiërarchie en werkprocessen.

Door deze workflow te volgen, kunnen we efficiënt samenwerken en de kwaliteit van onze code waarborgen.
