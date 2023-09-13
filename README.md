Svetainė pasiekiama adresu:
https://blogit-e6ho.onrender.com

Gali tekti kiek palaukti, kol svetainę pajungs pirmą kartą, kadangi naudojami free hostingai.
Galima kurtis naują paskyrą, arba prisijungti su sukurtu naudotoju: Username: Useris1 <br /> Password: Useris1! <br />

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
1. Prisijungti prie svetainės, todėl bus norima, kad naudotojas prisijungtų.

#### Registruotas naudotojas galės:
1. Atsijungti nuo svetainės,
2. Sukurti blogą:
  a. Suteikti paraštę,
  b. Aprašyti pasirinktą temą.
3.	Regaduoti savo blogą ar jį pašalinti,
4.	Komentuoti po savo ir kitų blogais,
5.	Keisti savo komentus ar juos šalinti,
6.	Paspausti „patinka“ arba „nepatinka“ ant komentarų,
7.	Pasikeisti iš tamsaus režimo į šviesų arba atvirkščiai.

#### Administratorius galės:
1.	Tą patį, ką gali bet kuris registruotas naudotojas,
2.	Šalinti bei keisti visų vartotojų blogus, komentarus.  

## Sistemos architektūra

  Sistema susideda iš dviejų dalių:
- Kliento pusė (angl. *Front-End*) – naudojamas *React*;
- Serverio pusė (angl. *Back-End*) – naudojamas *.NET Core*, duomenų bazė –
*MySQL*.
  Apačioje pateikta sistemos diegimo diagrama. Serverio pusė patalpinta *Azure*
serveryje, o visos jos komponentės diegiamos tame pačiame serveryje. Kliento pusė patalpinta *Render* serveryje. Aplikacija pasiekiama iš
vartotojo įrenginio naudojant *HTTPS* protokolą.

![image](https://user-images.githubusercontent.com/62296041/209277563-b07c4e47-c73e-4101-8ef5-5d151f31d9f4.png)

Likęs svetainės aprašymas patalpintas .pdf faile.
