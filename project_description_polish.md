<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Super Raport</title>
  <link rel="stylesheet" href="http://app.classeur.io/base-min.css" />
  <script type="text/javascript" src="https://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-AMS_HTML"></script>
</head>

<body>
  <div class="export-container">
<h1 id="wieś-spokojna-wieś-wesoła">Wieś spokojna, wieś wesoła</h1>
<h3 id="środowisko-dla-sztucznej-inteligencji.">Środowisko dla sztucznej inteligencji.</h3>
<h4 id="tymoteusz-kaczorowski-michał-moczulski-piotr-pietrzak">Tymoteusz Kaczorowski, Michał Moczulski, Piotr Pietrzak</h4>
<hr>
<h1 id="wstęp">Wstęp</h1>
<p>Przygotowaliśmy środowisko do testowania i implementacji sztucznej inteligencji oraz przykładowego agenta, na którego przykładzie można obejrzeć przygotowany interfejs i który pozwala bez dodatkowego nakładu pracy zobaczyć symulację w akcji. Wzorowaliśmy się na klasycznych strategiach czasu rzeczywistego, postawiliśmy jednak na indywidualność jednostek. Każda z nich posiada takie samo AI, jednak jego instancja jest całkowicie niezależna od innych (co nie wyklucza użycia, chociażby, użycia pól statycznych by zapewnić agentom wspólną jaźń).</p>
<h1 id="środowisko">Środowisko</h1>
<h2 id="obsługa-aplikacji">Obsługa aplikacji</h2>
<p>Na ekranie startowym należy wybrać plik .dll zawierający implementację interfejsu <code>IBrain</code> oraz nazwę klasy, która go implementuje. Część zasad świata (jak zasięg interakcji) można zmienić w pliku konfiguracyjnym. Po naciśnięciu przycisku “Let’s roll” rozpocznie się symulacja. Kliknięcie na dowolny <strong>Byt</strong> pozwala zobaczyć jego nazwę, stan oraz ID.</p>
<p>Jeśli zaznaczonym <strong>Bytem</strong> jest <strong>Wieśniak</strong>, możemy zasugerować jego mózgowi rozkaz, pisząc w konsoli:</p>
<ul>
<li><code>[walk/run/sneak] X Y</code></li>
<li><code>[gather/pickup/attack/drop/eat] (ID Bytu)</code></li>
<li><code>picktool [Axe/Sword/Pole] (ID wioski)</code></li>
<li><code>magazinepush (ID wioski) (ID zasobu)</code></li>
<li><code>magazinepull (ID wioski) [Wood/Food]</code></li>
<li><code>procreate (ID wioski)</code><br>
Jeśli włączone są oszustwa (cheaty) możemy, zaznaczywszy dowolny <strong>Byt</strong> wpisać:<br>
*<code>takedamage X</code><br>
*<code>getgathered</code></li>
</ul>
<h2 id="zasady-rządzące-światem">Zasady rządzące światem</h2>
<p>Świat jest kwadratem o rozmiarach 600x600, współrzędne x oraz y od -300 do 300. Żyją w nim różnego rodzaju <strong>Byty</strong>.</p>
<p>Wszystkie <strong>Byty</strong> posiadają jeden z typów:</p>
<ul>
<li><strong>Drzewo</strong></li>
<li><strong>Drewno</strong></li>
<li><strong>Jezioro</strong></li>
<li><strong>Jedzenie</strong></li>
<li><strong>Wieśniak</strong></li>
<li><strong>Tygrys</strong></li>
<li><strong>Wioska</strong></li>
</ul>
<p>Każdy przynależy również do jakiejś frakcji:</p>
<ul>
<li><strong>Czerwoni</strong></li>
<li><strong>Niebiescy</strong></li>
<li><strong>Mięsożercy</strong></li>
<li><strong>Neutralni</strong></li>
</ul>
<p>W świecie zawsze znajdują się dwie wioski należące do różnych frakcji (czerwonej lub niebieskiej). Gra kończy się zwycięstwem jednej frakcji, gdy wioska drugiej zostanie zniszczona.</p>
<h3 id="zasoby-jedzenie-drewno">Zasoby (jedzenie, drewno)</h3>
<p>Ich przeznaczeniem jest być przez Wieśniaków podnoszonym, upuszczanym, umieszczanym w magazynie wioski oraz jedzonym. Drewno nie posiada jednak żadnych wartości odżywczych (ludzie nie trawią celulozy).</p>
<h3 id="wioska">Wioska</h3>
<p>Tworzy nowych <strong>Wieśniaków</strong> i kolekcjonuje w magazynach przynoszone przez nich <strong>drewno</strong> (którego odpowiednia ilość skutkuje zwiększeniem jej poziomu) oraz <strong>jedzenie</strong>.<br>
Poziom <strong>Wioski</strong> wpływa na pojemność magazynów, jakość narzędzi oraz limit populacji.<br>
Każdy nowy <strong>Wieśniak</strong> może (w zależności od ustawień) mieć losowe cechy i nowy mózg, lub powstawać wskutek skrzyżowania pozostawionego przez innych <strong>Wieśniaków</strong> materiału genetycznego.<br>
Narodziny przez krzyżowanie wymagają dostępności materiału genetycznego przynajmniej dwóch <strong>Wieśniaków</strong> różnej płci. Materiał genetyczny jest jednorazowy. W przypadku wielu możliwych rodziców w pierwszej kolejności wykorzystywany jest najstarszy materiał genetyczny.<br>
Populacja startowa jest zawsze losowa.</p>
<h3 id="źródła-zasobów-drzewo-jezioro">Źródła zasobów (drzewo, jezioro)</h3>
<p>Ich przeznaczeniem jest być przez <strong>Wieśniaków</strong> zbieranym, co skutkuje pojawieniem się <strong>drewna</strong>/<strong>jedzenia</strong>.</p>
<h3 id="byty-żywe-tygrys-wieśniak">Byty żywe (tygrys, Wieśniak)</h3>
<p><strong>Byty</strong> żywe (w tym <strong>Wieśniacy</strong>) posiadają dwie istotne cechy: <em>życie</em> oraz <em>energię</em>, wyrażane w liczbach naturalnych. Spadek którejkolwiek z nich do zera oznacza śmierć.<br>
<em>Życie</em> spada wskutek zostania zaatakowanym przez inny <strong>Byt</strong> żywy, <em>energia</em> przez wykonanie czynności.<br>
<strong>Tygrysy</strong> nie męczą się, nie jedzą, nie zbierają, oraz nie utrzymują ekwipunku, a jedynie poruszają się, ryczą i atakują.</p>
<p>Każdy z <strong>Bytów</strong> żywych posiada trzy zmysły:</p>
<ul>
<li>Wzrok, który udostępnia opisy wszystkich widocznych <strong>Bytów</strong></li>
<li>Słuch, który udostepnia treść wypowiedzi w określonym zasięgu</li>
<li>“Czucie”, które udostępnia szczegółowy opis tego konkretnego <strong>Bytu</strong></li>
</ul>
<p><strong>Byty</strong> żywe potrafią wykonywać poniższe akcje:</p>
<ul>
<li>Czekanie</li>
<li>Poruszanie się
<ul>
<li>Chód: kosztuje 1 energii</li>
<li>Skradanie się: kosztuje 1 energii i jest dwa razy wolniejsze, ale utrudnia wykrycie przez inne <strong>Byty</strong></li>
<li>Bieg: kosztuje 2 energii i jest dwa razy szybsze</li>
</ul>
</li>
<li>Rozmowa (wszystkie byty w zasięgu słuchu otrzymują mówiony komunikat)
<ul>
<li>Mówienie: kosztuje 2 energii</li>
<li>Krzyk: kosztuje 4 energii i jest słyszalne na czterokrotnie większym dystansie</li>
</ul>
</li>
<li>Interakcja z innymi bytami
<ul>
<li>Atak: kosztuje 3 energii, zmniejsza zdrowie <strong>Bytu</strong> będącego celem</li>
<li>Zebranie: kosztuje 3 lub 7 energii (zależnie, czy mamy dobre narzędzie), skutkuje upuszczeniem przez cel na ziemię zasobów, jeśli jest źródłem zasobów</li>
<li>Podniesienie: umieszcza <strong>Byt</strong> w ekwipunku, jeśli ten jest zasobem</li>
<li>Upuszczenie: usuwa <strong>Byt</strong> z ekwipunku</li>
<li>Zjedzenie: zwiększa życie i energię, jeśli cel jest zasobem, w oparciu o jego wartość energetyczną.</li>
</ul>
</li>
</ul>
<p>Poruszanie się jest tym wolniejsze, im więcej przedmiotów <strong>Byt</strong> posiada w ekwipunku. Nie ma istnieje limit na liczbę niesionych przedmiotów.</p>
<p><strong>Byt</strong> wykonujący jedną z poniższych akcji traktujemy tak samo jak <strong>Byt</strong> skradający się:</p>
<ul>
<li>Czekanie</li>
<li>Mówienie</li>
<li>Podnoszenie</li>
<li>Upuszczanie</li>
<li>Jedzenie</li>
</ul>
<h4 id="efekty-akcji-na-bytach">Efekty akcji na Bytach</h4>
<p>Zaatakowanie <strong>drzewa</strong> niszczy je nie zostawiając żadnego <strong>drewna</strong>.<br>
Zaatakowanie <strong>jeziora</strong> nie ma efektu.<br>
Zaatakowanie zasobu niszczy go bezpowrotnie.<br>
Zebranie <strong>Bytu</strong>, który nie jest źródłem zasobu (<strong>jeziorem</strong> lub <strong>drzewem</strong>) nie ma efektu.<br>
Próba zjedzenia <strong>Bytu</strong>, który nie jest zasobem nie ma efektu.<br>
Podniesienie <strong>Bytu</strong>, który nie jest zasobem nie ma efektu.</p>
<h4 id="interakcje-z-wioską">Interakcje z Wioską</h4>
<p>Dodatkowo <strong>Wieśniacy</strong> mogą nawiązać następujące interakcje z <strong>Wioską</strong>:</p>
<ul>
<li>Umieszczenie zasobu w magazynie</li>
<li>Wyjęcie zasobu z magazynu</li>
<li>Wybór narzędzia</li>
<li>Umieszczenie swojego materiału genetycznego w banku, by mogło urodzić się dziecko dziedziczące jego cechy</li>
</ul>
<p>Przy żadnej z nich nie traktujemy <strong>Wieśniaka</strong> jako skradającego się. Jeśli celem takiej interakcji nie jest <strong>Wioska</strong> – nie ma ona efektu</p>
<p><strong>Wioska</strong> nie sprawdza, czy wchodzący z nią w interakcję <strong>Wieśniak</strong> jest sojusznikiem, czy wrogiem. Oznacza to, że kradzieże i gwałty są możliwe.</p>
<h4 id="cechy-indywidualne-bytów">Cechy indywidualne Bytów</h4>
<p>Czas i skala efektu pewnych czynności może zależeć od indywidualnych cech <strong>Bytu</strong>.</p>
<p>Każdy <strong>Wieśniak</strong> posiada trzy cechy, które wpływają na jego możliwości:</p>
<ul>
<li><em>Siła</em>
<ul>
<li>Zwiększa obrażenia zadawane w walce.</li>
<li>Zwiększa maksymalną energię.</li>
<li>Zmniejsza czas potrzebny na ścięcie <strong>drzewa</strong>.</li>
</ul>
</li>
<li><em>Zręczność</em>
<ul>
<li>Zwiększa skuteczność skradania się.</li>
<li>Zwieksza szybkość poruszania się.</li>
<li>Zmniejsza czas między atakami.</li>
<li>Zmniejsza czas potrzebny na złowienie ryby.</li>
</ul>
</li>
<li><em>Inteligencja</em>
<ul>
<li>Zmniejsza czas mówienia.</li>
<li>Zwiększa zasięg wzroku.</li>
</ul>
</li>
</ul>
<h4 id="narzędzia">Narzędzia</h4>
<p>Każdy <strong>Wieśniak</strong> może nosić na raz jedno narzędzie wybrane w wiosce. Jakość narzędzia (określająca korzyści, jakie przynosi) zależy od poziomu <strong>Wioski</strong> i nie podnosi się automatycznie, gdy <strong>Wioska</strong> awansuje (należy wrócić do niej i wybrać to narzędzie ponownie). Są to:</p>
<ul>
<li><em>Miecz</em>, który zwiększa zadawane obrażenia.</li>
<li><em>Siekiera</em>, która w mniejszym stopniu zwiększa zadawane obrażenia oraz pozwala bardziej efektywnie ścinać drzewa.</li>
<li><em>Wędka</em>, która pozwala bardziej efektywnie łowić ryby.</li>
</ul>
<h2 id="opis-interfejsu-programistycznego-namespace-aiprotocol">Opis interfejsu programistycznego (namespace <code>AiProtocol</code>)</h2>
<p>Struktura <code>VillageRules</code> – służy do przekazywania stałych ustalanych w grze, będących zasadami funkcjonowania świata, takimi jak maksymalny zasięg, na którym da się nawiązać interakcję</p>
<p>Namespace <code>Descriptions</code> – zawiera definicje opisów <strong>Bytów</strong> występujących w świecie</p>
<ul>
<li><code>ResourceType</code> – definiuje możliwe rodzaje zasobów: <strong>drewno</strong> i <strong>jedzenie</strong></li>
<li><code>EntityType</code> – definiuje możliwe rodzaje <strong>Bytów</strong>, <strong>Byty</strong> o tym samym <code>EntityType</code> rozróżnialne są jedynie przez cechy indywidualne</li>
<li><code>Faction</code> – definiuje dostępne frakcje (drużyny), do których przynależą <strong>Byty</strong></li>
<li><code>ToolKind</code> – definiuje rodzaje narzędzi, które może nosić ze sobą <strong>Wieśniak</strong></li>
<li><code>BaseDescription</code> – klasa abstrakcyjna stanowiąca podstawę dla wszystkich innych opisów, określająca informacje, które są dostępne na temat każdego <strong>Bytu</strong></li>
<li><code>LivingDescription</code> – określa informacje, które są dostępne o <strong>Bytach</strong> żywych, jak <strong>tygrys</strong> czy <strong>Wieśniak</strong></li>
<li><code>VillageDescription</code> – określa informacje, które dostępne są na temat <strong>Wiosek</strong></li>
<li><code>ResourceNodeDescription</code> – informacje dostępne na temat <strong>drzew</strong> i <strong>jezior</strong></li>
<li><code>ResourceType</code> – informacje dostępne na temat <strong>jedzenia</strong> i <strong>drewna</strong></li>
<li><code>BodilyFunctions</code> – szczegółowe informacje opisujące <strong>Byt</strong> żywy, dostępne dla <strong>Agenta</strong> kontrolującego ten <strong>Byt</strong></li>
</ul>
<p>Namespace <code>Command</code> – zawiera definicje rozkazów, które mózg może wydać ciału<br>
	<code>MovementStyle</code> – definiuje możliwe sposoby poruszania się (bieg/chód/skradanie się)<br>
	<code>TalkStyle</code> – definiuje style wypowiedzi (mowa/krzyk)<br>
	<code>InteractionStyle</code> – definiuje rodzaje interakcji z innymi bytami (atak/zebranie/podniesienie/upuszczenie/zjedzenie)<br>
	<code>Mood</code> – definiuje możliwe nastroje w jakich Byt jest wykonując dane polecenie<br>
	<code>BaseCommand</code> – klasa abstrakcyjna stanowiąca podstawę dla wszystkich innych rozkazów<br>
	<code>Movement</code> – opisuje rozkaz ruchu: sposób poruszania się oraz koordynaty celu<br>
	<code>Talk</code> – opisuje rozkaz rozmowy: styl wypowiedzi oraz jej treść w postaci obiektu typu <code>Words</code><br>
	<code>Interaction</code> – opisuje rozkaz interakcji: rodzaj interakcji oraz ID celu<br>
	<code>Wait</code> – opisuje pusty rozkaz oznaczający stanie w miejscu<br>
	<code>VillageInteraction</code> – klasa abstrakcyjna opisująca rozkaz interakcji z <strong>Wioską</strong>, zawiera cel w postaci ID <strong>Wioski</strong><br>
	<code>MagazinePush</code> – opisuje rozkaz włożenia czegoś do magazynu, konkretnie ID wkładanego zasobu<br>
	<code>MagazinePull</code> – opisuje rozkaz wyjęcia czegoś z magazynu, konkretnie <code>ResourceType</code> żądanego zasobu<br>
	<code>Procreate</code> – opisuje rozkaz umieszczenia w banku <strong>Wioski</strong> materiału genetycznego <strong>Wieśniaka</strong> (jego cech fizycznych oraz cech mózgu, jeśli zdefiniowaliśmy odpowiednie <code>IBrainGenetics</code>)</p>
<p>Klasa abstrakcyjna <code>Words</code> – opisuje podstawę, którą powinny implementować wszystkie treści, które chcemy, by <strong>Wieśniacy</strong> mogli mówić i słyszeć. Tworząc obiekt tego typu nie musimy wypełniać pól zdefiniowanych w samym <code>Words</code> - zajmie się tym gra.<br>
klasa <code>Roar</code> – przykładowa wypowiedź dziedzicząca po <code>Words</code>, będąca jedyną rzeczą wypowiadaną przez <strong>tygrysy</strong></p>
<p>Interfejs <code>IBrain</code> – definiuje kontrakt, który musi spełniać agent sztucznej inteligencji <strong>Wieśniaka</strong>.<br>
Metody intefejsu:</p>
<ul>
<li><code>void SetRules(GameRules)</code> - wywoływane po stworzeniu nowego mózgu. Argumentem są zasady rządzące światem.</li>
<li><code>void SetNextAction(BaseCommand)</code> - wywoływane, gdy gracz zasugeruje za pośrednictwem konsoli jakąś czynność. Można ją zignorować, albo wykonać. Argumentem jest sugerowana czynność.</li>
<li><code>void Hear(Words)</code> - wywoływane ilekroć <strong>Wieśniak</strong> agenta usłyszy jakąś wypowiedź.</li>
<li><code>void See(IEnumerable&lt;BaseDescription&gt;)</code> - wywoływane przed zapytaniem o każdą kolejną akcję, argumentem jest kolekcja opisów <strong>Bytów</strong> w zasięgu wzroku.</li>
<li><code>void Feel(BodilyFunctions)</code> - wywoływane przed zapytaniem o każdą kolejną akcję, argumentem jest szczegółowy opis stanu kontrolowanego <strong>Wieśniaka</strong>.</li>
<li><code>IEnumerable&lt;BaseCommand&gt; GetDecisions()</code> - powinno być leniwą, nieskończoną kolekcją rozkazów, gdzie przed ustaleniem każdego z nich analizujemy dane pochodzące od zmysłów.</li>
<li><code>void Initialize(BrainGenetics)</code> - służy do inicjalizowania stanu mózgu w oparciu o materiał genetyczny, który sami definiujemy. Jeśli nie zależy nam na dziedziczeniu cech mózgu metoda ta może nic nie robić.</li>
<li><code>BrainGenetics GetGeneticMaterial()</code> - powinna zwracać materiał genetyczny reprezentujący ten mózg lub null, jeśli nie zależy nam na dziedziczeniu cech mózgu.</li>
</ul>
<p>Interfejs <code>IBrainGenetics</code> – powinna go implementować klasa reprezentująca cechy charakterystyczne mózgu, jeśli chcemy by nowo narodzeni <strong>Wieśniacy</strong> dziedziczyli je wskutek akcji <code>Procreate</code>.<br>
Metody interfejsu:</p>
<ul>
<li><code>IBrainGenetics Cross(IBrainGenetics)</code> – powinna zwracać wynik krzyżowania materiałów genetyczny pochodzących od dwójki rodziców. Argumentem zawsze będzie materiał ojca, a metoda wywoływana będzie na materiale matki.</li>
</ul>
<h1 id="przykładowy-agent">Przykładowy agent</h1>
<p>Załączony kod przykładowego agenta pokazuje, jak zaimplementować interfejs <code>IBrain</code> i w prosty sposób dostarczyć swoją sztuczną inteligencję do gry. Dzięki niemu możemy też zobaczyć, jak w praktyce wygląda życie Wieśniaków. Agent jest oparty na drzewach decyzyjnych z elementami maszyny stanów.</p>
<h1 id="podsumowanie">Podsumowanie</h1>
<p>Semestr jest zbyt krótki by zrealizować w jego trakcie tak poważny projekt. Wiele elementów zmuszeni byliśmy tylko poruszyć, jeszcze więcej jedynie zauważyć. Poniżej podajemy listę naszych pomysłów na dalsze zgłębianie zagadnienia, w nadziei, że kiedyś ktoś podejmie się kontynuować nasze epokowe dzieło.</p>
<h2 id="plany">Plany</h2>
<ul>
<li>Agent oparty na uczeniu przez wzmacnianie, sieć neuronowa</li>
<li>Ewolucja AI, równoległa z biologiczną</li>
<li>Agent oparty na dynamicznie zmieniającym się drzewie behawioralnym</li>
</ul>
<h2 id="możliwe-poprawki">Możliwe poprawki</h2>
<ul>
<li>Rozszerzenie środowiska:
<ul>
<li>Więcej urozmaiceń terenu</li>
<li>Rozbicie rozwoju <strong>Wioski</strong> na różne aspekty (poziom umocnień, poziom kuźni, etc.)</li>
<li>Bardziej realne, produkowalne przedmioty</li>
<li>Różnorodne materiały budowlane i źródła pożywienia</li>
<li>Więcej postaci niezależnych (np. kozy dające mleko)</li>
</ul>
</li>
<li>Ulepszenie przykładowego agenta</li>
<li>Serializacja stanu symulacji</li>
<li>Zwiększenie wygody obserwowania symulacji: lista <strong>Wieśniaków</strong> z możliwością skakania między nimi kamerą, dodatkowe informacje o <strong>Bytach</strong></li>
<li>Możliwość przydzielania różnych AI obu <strong>Wioskom</strong></li>
</ul>
<h1 id="podział-prac-w-zespole">Podział prac w zespole</h1>
<h3 id="tymoteusz-kaczorowski">Tymoteusz Kaczorowski</h3>
<ul>
<li>Interakcja ze światem gry</li>
<li>Funkcjonowanie gameplayu</li>
<li>Rozwój protokołu</li>
<li>Interfejs graficzny</li>
<li>Grafika</li>
</ul>
<h3 id="michał-moczulski">Michał Moczulski</h3>
<ul>
<li>Pierwsza wersja protokołu</li>
<li>Przykładowy agent</li>
<li>Agent sterujący <strong>Tygrysem</strong></li>
</ul>
<h3 id="piotr-pietrzak">Piotr Pietrzak</h3>
<ul>
<li>Możliwość edycji i konfiguracji zasad środowiska</li>
<li>Logika zarządzania rzeczywistością (Managery)</li>
<li>Rozszerzenie edytora</li>
<li>Interfejs użytkownika</li>
<li>Grafika</li>
</ul></div>
</body>

</html>
