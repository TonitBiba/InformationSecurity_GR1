#InformationSecurity_GR1
https://github.com/TonitBiba/Information_Security_UP <br>
Aplikacioni i zhvilluar ne kuader të detyres se pare në lëndën "Siguria e Informacionit". Ky aplikacion është zhvilluar për të dekriptuar dhe nxjerrë çelësin që është përdorur për enkriptim në kuadër të detyrë së parë në lëndën "Siguria e Informacionit". Aplikacioni është zhvilluar në platëformën desktop WPF, në gjuhën programueses C#, duke përdorur editorin Visual Studio 2019. Fillimisht është zhvilluar për detyrën e grupit 1, pastaj është gjeneralizuar edhe për detyrat e grupeve të tjera, që do të thotë se ky aplikacion mund të përdoret edhe për dekriptimin e detyrave tjera.
1. Udhëzimet për egzekutimin e programit:<br>
  Preferohet që programi të egzekutohet duke instaluar aplikacionin që ndodhet në kuadër të folder-it <b>Setup</b>
  Instalimi dhe ekzekutimi i këtij programi është pothuasje i njejtë me programet e zakonshme që instalohen në kompjuter 
<hr>
2. Mënyra e gjetjes së çelësit dhe dekriptimit të mesazhit <br>
  Procesi i gjetjes së çelësit dhe mesazhit të dekriptuar kalon në disa hapa, ashtu siq edhe është treguar përmes kodit. Këta hapa janë:
    <ul>
      <li> Ngarkohet lista e çelësave, të marrë nga fajlli keys.txt;</li>
      <li> Filtrohen lista e çelësave duke përdorur të dhënat statistikore, ashtu që nga <b>100000</b> sa jane gjithsejt në fajll, mbeten vetëm <b>3</b> çelësa.</li>
      <li> 3 çelësat e gjeneruar janë me madhësi <b>14 byte</b>, duke marrë parasysh faktin se këto të dhëna janë enkriptuar me AES 128 bit, madhësia prej 14 byte nuk është çelës i mjaftueshëm për të dekriptuar këto të dhëna. 
        Pasiqë 14 byte = 112 bit, kjo do të thotë se nevojiten edhe 16 bit-a për të plotësuar çelësin 128 bit-ësh, të nevojshëm për dekriptim.
      </li>
      <li>
        Për çdo çelësi të gjeneruar shikohet se është çelësi i duhur. Për të shikuar se a është çelësi i duhur përdoret kushti nëse teksti i
        dekriptuar përmban pjesen <b>"KODI"</b>, në rast se egziston kjo fjali atëherë ky konsiderohet se është çelësi që është përdorur me rastin e
        enkriptimit të mesazhit.
      </li>
      <li>
        Testimi se a është çelësi i duhur për dekriptim është bërë duke përdorur klasat e gatshme të DotNet-it, <b>RijndaelManaged</b>. 
      </li>      
    </ul>
   <hr/>
3. Analiza teorike - Numri i kombinimeve dhe hapsira e çelësave <br>
  Pas filtrimit të çelësave ashtu siq është spjeguar në pikën 2, Mbeten vetëm 3 çelësa 14 byte që kanë plotësuar kriteret e kërkimit.
  Numri total i kombinimeve që mund të fitohet nga 3 çelësat e mbetur është: <b> 3 * 65,536 =  196,608 </b>.
  Në rastin më të keq të mundshëm duhet të përdoren gjitsejt 196,608 çelësat e mundshëm për të arritur në rezultatin e kërkuar. 
  çelësat gjenerohen nga programi i krijuar, duke u hyrë fillimisht në një unazë foreach të 3 çelësave të gjeneruar, pastaj një unazë tjetër
  e for që iteron nga 0 deri në 65535, përdoret për të gjeneruar 2 byte-at e përmendur në pikën 2.
<hr/>
Nese ekzekutohet aplikacionit per detyren e pare, rezultati do te jete:
<ul>
  <li><b>Celësi: 5E15F4E6314191DCF6401A6AFBF6BA08</b>;</li>
  <li><b>Teksti i dekriptuar: GR 01 KODI 94945</b>.</li>
 </ul>
