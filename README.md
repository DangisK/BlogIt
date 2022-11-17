# BlogIt
Blogų kūrimo svetainė, skirta T120B165 Saityno taikomųjų programų projektavimo modulio laboratoriniams realizuoti. 

## Sprendžiamo užvadinio aprašymas

### Sistemos paskirtis

  *Blogas* – tai lankstaus kurinio puslapis, kuriame žmonės gali publikuoti informacinius
skelbimus arba kurti diskusijas.

  Kuriamos sistemos tikslas – leisti žmonėms kurti internetinius *blogus*, kuriuos galėtų
matyti kiti vartotojai. Taip pat naudotojai turėtų galimybę juos komentuoti, ar atsakyti į jiems
skirtus komentarus.

  Norint naudotis svetaine, nebūtina susikurti paskyros. Neregistruoti svečiai gali naršyti
kitų vartotojų *blogus*, peržiūrėti komentarus ar atsakus į juos, tačiau norint išnaudoti svetainės
pilną funkcionalumą, reikėtų paskyrą susikurti. Tai suteiktų galimybę kurti *blogus*, dėti patiktukus ant komentarų bei komentuoti kitų *blogus*.

### Funkciniai reikalavimai

#### Neregistruotas sistemos naudotojas galės:
1. Prisijungti prie svetainės,
2. Peržiūrėti kitų vartotojų blogus,
3. Matyti komentarus ir reakcijas į juos,
4. Matyti vartotojų sąrašą.

#### Registruotas naudotojas galės:
1. Atsijungti nuo svetainės,
2. Sukurti blogą:
  a. Suteikti paraštę,
  b. Aprašyti pasirinktą temą.
3. Regaduoti blogą,
4. Šalinti blogą,
5. Komentuoti po savo ir kitų blogais,
6. Paspausti patinka ar nepatinka ant komentaro,
7. Keisti paskyros nustatymus.

#### Administratorius galės:
1. Matyti vartotojų sąrašą,
2. Peržiūrėti vartotojų blogus,
3. Šalinti blogus,
4. Šalinti vartotojus.

## Sistemos architektūra

  Sistema susideda iš dviejų dalių:
- Kliento pusė (angl. *Front-End*) – naudojamas *React*;
- Serverio pusė (angl. *Back-End*) – naudojamas *.NET Core*, duomenų bazė –
*MySQL*.
  Apačioje pateikta sistemos diegimo diagrama. Sistema bus patalpinta *Azure*
serveryje, o visos jos komponentės diegiamos tame pačiame serveryje. Aplikacija pasiekiama iš
vartotojo įrenginio naudojant *HTTP* protokolą.

![api](https://user-images.githubusercontent.com/62296041/197076248-6162695b-9ead-496c-a6a3-fff4a7238648.png)

