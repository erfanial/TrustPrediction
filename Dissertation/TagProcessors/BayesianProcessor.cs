﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dissertation.TagProcessors
{
    class BayesianProcessor
    {
        DataSimulation.SectionOccupancyValueToTagPolicy policy = new DataSimulation.SectionOccupancyValueToTagPolicy();
        ExperimentInputParams param = new ExperimentInputParams();


        /////// see createBayesianLookups.m

        ////// 5 tags
        ////private double[] probabilityOfObservation = new double[] { 0.0390491366788622, 0.0484109691244110, 0.0702569575594639, 0.154186534157200, 0.414089085911369, 0.154186534157200, 0.0702569575594639, 0.0484109691244110, 0.0390491366788622 };
        ////private double[,] probability_observationGivenTrust = new double[,] { { 0.111148687877349, 0.111148714651381, 0.111148733775695, 0.111148745250286, 0.111148749075150, 0.111148745250286, 0.111148733775695, 0.111148714651381, 0.111148687877349 }, { 0.111263123589407, 0.111263566154081, 0.111263882273225, 0.111264071945319, 0.111264135169451, 0.111264071945319, 0.111263882273225, 0.111263566154081, 0.111263123589407 }, { 0.111455845431478, 0.111458160913034, 0.111459814869931, 0.111460807260670, 0.111461138060350, 0.111460807260670, 0.111459814869931, 0.111458160913034, 0.111455845431478 }, { 0.111726557985918, 0.111734123468726, 0.111739527826770, 0.111742770618313, 0.111743851578282, 0.111742770618313, 0.111739527826770, 0.111734123468726, 0.111726557985918 }, { 0.112072945431513, 0.112092045379753, 0.112105691004988, 0.112113879502416, 0.112116609188632, 0.112113879502416, 0.112105691004988, 0.112092045379753, 0.112072945431513 }, { 0.112490338318391, 0.112531301234271, 0.112560573303708, 0.112578141685446, 0.112583998669627, 0.112578141685446, 0.112560573303708, 0.112531301234271, 0.112490338318391 }, { 0.112971347596301, 0.113049842347003, 0.113105956941601, 0.113139644482331, 0.113150876795014, 0.113139644482331, 0.113105956941601, 0.113049842347003, 0.112971347596301 }, { 0.113505472952876, 0.113643968906669, 0.113743039786832, 0.113802540490841, 0.113822383764482, 0.113802540490841, 0.113743039786832, 0.113643968906669, 0.113505472952876 }, { 0.114078698427977, 0.114308083205840, 0.114472325258136, 0.114571029234384, 0.114603957060591, 0.114571029234384, 0.114472325258136, 0.114308083205840, 0.114078698427977 }, { 0.114673095806794, 0.115034429083653, 0.115293499795470, 0.115449334092403, 0.115501344336845, 0.115449334092403, 0.115293499795470, 0.115034429083653, 0.114673095806794 }, { 0.115266465343575, 0.115812825889101, 0.116205298561023, 0.116441673889247, 0.116520614856850, 0.116441673889247, 0.116205298561023, 0.115812825889101, 0.115266465343575 }, { 0.115832053473813, 0.116630409109401, 0.117205360905377, 0.117552228521344, 0.117668168909461, 0.117552228521344, 0.117205360905377, 0.116630409109401, 0.115832053473813 }, { 0.116338397394411, 0.117471394177834, 0.118290078233797, 0.118785098042727, 0.118950744519851, 0.118785098042727, 0.118290078233797, 0.117471394177834, 0.116338397394411 }, { 0.116749355182992, 0.118316884601592, 0.119454438229694, 0.120144254708838, 0.120375420663351, 0.120144254708838, 0.119454438229694, 0.118316884601592, 0.116749355182992 }, { 0.117024385299239, 0.119144749964494, 0.120691870904042, 0.121633487607210, 0.121949616070390, 0.121633487607210, 0.120691870904042, 0.119144749964494, 0.117024385299239 }, { 0.117119138154317, 0.119929602880154, 0.121994103570143, 0.123256339689387, 0.123681082591522, 0.123256339689387, 0.121994103570143, 0.119929602880154, 0.117119138154317 }, { 0.116986412092277, 0.120642905717398, 0.123351033474057, 0.125016037268392, 0.125577891977936, 0.125016037268392, 0.123351033474057, 0.120642905717398, 0.116986412092277 }, { 0.116577504262868, 0.121253236883722, 0.124750628269611, 0.126915412363938, 0.127648414834311, 0.126915412363938, 0.124750628269611, 0.121253236883722, 0.116577504262868 }, { 0.115843952583417, 0.121726741650391, 0.126178865588046, 0.128956818662456, 0.129901290429611, 0.128956818662456, 0.126178865588046, 0.121726741650391, 0.115843952583417 }, { 0.114739619827821, 0.122027783202649, 0.127619723350191, 0.131142042302805, 0.132345386023138, 0.131142042302805, 0.127619723350191, 0.122027783202649, 0.114739619827821 }, { 0.113223019531587, 0.122119795607631, 0.129055231924373, 0.133472209184053, 0.134989744397227, 0.133472209184053, 0.129055231924373, 0.122119795607631, 0.113223019531587 }, { 0.111259733696884, 0.121966322339094, 0.130465597496944, 0.135947690990759, 0.137843518406926, 0.135947690990759, 0.130465597496944, 0.121966322339094, 0.111259733696884 }, { 0.108824734114917, 0.121532203516458, 0.131829402933139, 0.138568012603641, 0.140915891586246, 0.138568012603641, 0.131829402933139, 0.121532203516458, 0.108824734114917 }, { 0.105904402240838, 0.120784854724876, 0.133123887956168, 0.141331763957530, 0.144215984216483, 0.141331763957530, 0.133123887956168, 0.120784854724876, 0.105904402240838 }, { 0.102498053689732, 0.119695563473932, 0.134325304866258, 0.144236519663449, 0.147752744789725, 0.144236519663449, 0.134325304866258, 0.119695563473932, 0.102498053689732 }, { 0.0986188137099783, 0.118240719374924, 0.135409339709044, 0.147278769762975, 0.151534827509765, 0.147278769762975, 0.135409339709044, 0.118240719374924, 0.0986188137099783 }, { 0.0942937538073922, 0.116402893571538, 0.136351582470763, 0.150453864770654, 0.155570457373025, 0.150453864770654, 0.136351582470763, 0.116402893571538, 0.0942937538073922 }, { 0.0895632756215868, 0.114171692911239, 0.137128024380260, 0.153755977639385, 0.159867285456859, 0.153755977639385, 0.137128024380260, 0.114171692911239, 0.0895632756215868 }, { 0.0844798019979574, 0.111544333831544, 0.137715556629186, 0.157178084437450, 0.164432238281755, 0.157178084437450, 0.137715556629186, 0.111544333831544, 0.0844798019979574 }, { 0.0791058940135187, 0.108525906985504, 0.138092443545713, 0.160711964377226, 0.169271366447067, 0.160711964377226, 0.138092443545713, 0.108525906985504, 0.0791058940135187 }, { 0.0735119483473471, 0.105129331867984, 0.138238744928917, 0.164348218456937, 0.174389699071185, 0.164348218456937, 0.138238744928917, 0.105129331867984, 0.0735119483473471 }, { 0.0677736401863931, 0.101375026384104, 0.138136666878428, 0.168076304492537, 0.179791111763308, 0.168076304492537, 0.138136666878428, 0.101375026384104, 0.0677736401863931 }, { 0.0619692671095854, 0.0972903354663683, 0.137770827541754, 0.171884584897723, 0.185478216748620, 0.171884584897723, 0.137770827541754, 0.0972903354663683, 0.0619692671095854 }, { 0.0561771269568509, 0.0929087732348462, 0.137128432806699, 0.175760382416336, 0.191452284173089, 0.175760382416336, 0.137128432806699, 0.0929087732348462, 0.0561771269568509 }, { 0.0504730359808437, 0.0882691346205451, 0.136199365859448, 0.179690038325455, 0.197713203338864, 0.179690038325455, 0.136199365859448, 0.0882691346205451, 0.0504730359808437 }, { 0.0449280690662063, 0.0834145265166312, 0.134976202429413, 0.183658967579418, 0.204259491505619, 0.183658967579418, 0.134976202429413, 0.0834145265166312, 0.0449280690662063 }, { 0.0396065847387882, 0.0783913582524482, 0.133454169365173, 0.187651706056231, 0.211088355841574, 0.187651706056231, 0.133454169365173, 0.0783913582524482, 0.0396065847387882 }, { 0.0345645841846977, 0.0732483196782472, 0.131631067242819, 0.191651946500407, 0.218195811127336, 0.191651946500407, 0.131631067242819, 0.0732483196782472, 0.0345645841846977 }, { 0.0298484435269366, 0.0680353650987108, 0.129507177808713, 0.195642561815724, 0.225576852045902, 0.195642561815724, 0.129507177808713, 0.0680353650987108, 0.0298484435269366 }, { 0.0254940493464729, 0.0628027144085523, 0.127085174501374, 0.199605616818835, 0.233225674619654, 0.199605616818835, 0.127085174501374, 0.0628027144085523, 0.0254940493464729 }, { 0.0215263564481903, 0.0575998796383194, 0.124370049756599, 0.203522372101696, 0.241135937001769, 0.203522372101696, 0.124370049756599, 0.0575998796383194, 0.0215263564481903 }, { 0.0179593728140325, 0.0524747253153803, 0.121369067152865, 0.207373285903423, 0.249301045910283, 0.207373285903423, 0.121369067152865, 0.0524747253153803, 0.0179593728140325 }, { 0.0147965595025387, 0.0474725735574534, 0.118091740591223, 0.211138021509750, 0.257714452046618, 0.211138021509750, 0.118091740591223, 0.0474725735574534, 0.0147965595025387 }, { 0.0120316140056677, 0.0426353683742473, 0.114549837372092, 0.214795468403813, 0.266369936338102, 0.214795468403813, 0.114549837372092, 0.0426353683742473, 0.0120316140056677 }, { 0.00964958605236833, 0.0380009170623673, 0.110757397735908, 0.218323785036005, 0.275261869098646, 0.218323785036005, 0.110757397735908, 0.0380009170623673, 0.00964958605236833 }, { 0.00762825715113959, 0.0336022289166801, 0.106730760412698, 0.221700469673363, 0.284385426293089, 0.221700469673363, 0.106730760412698, 0.0336022289166801, 0.00762825715113959 }, { 0.00593970131347510, 0.0294669721681027, 0.102488581980149, 0.224902463507082, 0.293736750829863, 0.224902463507082, 0.102488581980149, 0.0294669721681027, 0.00593970131347510 }, { 0.00455193604700078, 0.0256170688374390, 0.0980518372048723, 0.227906287358658, 0.303313051749921, 0.227906287358658, 0.0980518372048723, 0.0256170688374390, 0.00455193604700078 }, { 0.00343057091807680, 0.0220684440704636, 0.0934437878112501, 0.230688210340269, 0.313112639694275, 0.230688210340269, 0.0934437878112501, 0.0220684440704636, 0.00343057091807680 }, { 0.00254036611222154, 0.0188309416811916, 0.0886899080688245, 0.233224446124401, 0.323134902400268, 0.233224446124401, 0.0886899080688245, 0.0188309416811916, 0.00254036611222154 }, { 0.00184662506003920, 0.0159084113993313, 0.0838177570550459, 0.235491370439802, 0.333380228517893, 0.235491370439802, 0.0838177570550459, 0.0159084113993313, 0.00184662506003920 }, { 0.00131636221885453, 0.0132989661098232, 0.0788567893559986, 0.237465752296855, 0.343849891224579, 0.237465752296855, 0.0788567893559986, 0.0132989661098232, 0.00131636221885453 }, { 0.000919207786604648, 0.0109953996738888, 0.0738380983009399, 0.239124991356053, 0.354545904657213, 0.239124991356053, 0.0738380983009399, 0.0109953996738888, 0.000919207786604648 }, { 0.000628033372275072, 0.00898574826926516, 0.0687940886091533, 0.240447354717296, 0.365470866050520, 0.240447354717296, 0.0687940886091533, 0.00898574826926516, 0.000628033372275072 }, { 0.000419304235999941, 0.00725397114260225, 0.0637580785757756, 0.241412208000411, 0.376627794899612, 0.241412208000411, 0.0637580785757756, 0.00725397114260225, 0.000419304235999941 }, { 0.000273182588074547, 0.00578072077549291, 0.0587638356086954, 0.242000237577323, 0.388019977867976, 0.242000237577323, 0.0587638356086954, 0.00578072077549291, 0.000273182588074547 }, { 0.000173420971876200, 0.00454416821306448, 0.0538450529508901, 0.242193662826180, 0.399650825052006, 0.242193662826180, 0.0538450529508901, 0.00454416821306448, 0.000173420971876200 }, { 0.000107093941812972, 0.00352084706646914, 0.0490347795955718, 0.241976438949954, 0.411523740095761, 0.241976438949954, 0.0490347795955718, 0.00352084706646914, 0.000107093941812972 }, { 6.42197873028316e-05, 0.00268647969927072, 0.0443648194565034, 0.241334451954126, 0.423642003939031, 0.241334451954126, 0.0443648194565034, 0.00268647969927072, 6.42197873028316e-05 }, { 3.73223463071734e-05, 0.00201675137714784, 0.0398651194605029, 0.240255707643048, 0.436008669942499, 0.240255707643048, 0.0398651194605029, 0.00201675137714784, 3.73223463071734e-05 }, { 2.09769713872365e-05, 0.00148800253589675, 0.0355631690213359, 0.238730515937936, 0.448626466863026, 0.238730515937936, 0.0355631690213359, 0.00148800253589675, 2.09769713872365e-05 }, { 1.13758111681708e-05, 0.00107781545208834, 0.0314834349869974, 0.236751670536250, 0.461497705597745, 0.236751670536250, 0.0314834349869974, 0.00107781545208834, 1.13758111681708e-05 }, { 5.93725043379540e-06, 0.000765478979023555, 0.0276468563455493, 0.234314622124259, 0.474624185622439, 0.234314622124259, 0.0276468563455493, 0.000765478979023555, 5.93725043379540e-06 }, { 2.97403227057162e-06, 0.000532323034914604, 0.0240704215647616, 0.231417641295355, 0.488007097416001, 0.231417641295355, 0.0240704215647616, 0.000532323034914604, 2.97403227057162e-06 }, { 1.42542063236899e-06, 0.000361922564861397, 0.0207668484212444, 0.228061965318862, 0.501646917693439, 0.228061965318862, 0.0207668484212444, 0.000361922564861397, 1.42542063236899e-06 }, { 6.51525578878926e-07, 0.000240178140660361, 0.0177443817163700, 0.224251921238619, 0.515543294813372, 0.224251921238619, 0.0177443817163700, 0.000240178140660361, 6.51525578878926e-07 }, { 2.82960839962095e-07, 0.000155286699254100, 0.0150067187206669, 0.219995016703488, 0.529694922190200, 0.219995016703488, 0.0150067187206669, 0.000155286699254100, 2.82960839962095e-07 }, { 1.16302714406760e-07, 9.76207661847821e-05, 0.0125530660107809, 0.215301989622953, 0.544099397891459, 0.215301989622953, 0.0125530660107809, 9.76207661847821e-05, 1.16302714406760e-07 }, { 4.50416894174684e-08, 5.95376267997970e-05, 0.0103783251072922, 0.210186808307343, 0.558753068845501, 0.210186808307343, 0.0103783251072922, 5.95376267997970e-05, 4.50416894174684e-08 }, { 1.63570610190586e-08, 3.51412077684334e-05, 0.00847339851621053, 0.204666615232344, 0.573650858257189, 0.204666615232344, 0.00847339851621053, 3.51412077684334e-05, 1.63570610190586e-08 }, { 5.54059916529310e-09, 2.00189700820156e-05, 0.00682560284840349, 0.198761609941548, 0.588786074973037, 0.198761609941548, 0.00682560284840349, 2.00189700820156e-05, 5.54059916529310e-09 }, { 1.74032563699498e-09, 1.09740778504361e-05, 0.00541917189355487, 0.192494869797841, 0.604150203696376, 0.192494869797841, 0.00541917189355487, 1.09740778504361e-05, 1.74032563699498e-09 }, { 5.03647515097382e-10, 5.76979665076887e-06, 0.00423582990418903, 0.185892111193550, 0.619732675168752, 0.185892111193550, 0.00423582990418903, 5.76979665076887e-06, 5.03647515097382e-10 }, { 1.33337428231768e-10, 2.89889028162654e-06, 0.00325541375075252, 0.178981398248952, 0.635520615745244, 0.178981398248952, 0.00325541375075252, 2.89889028162654e-06, 1.33337428231768e-10 }, { 3.20393902061562e-11, 1.38619417821915e-06, 0.00245652175860172, 0.171792810709891, 0.651498576238915, 0.171792810709891, 0.00245652175860172, 1.38619417821915e-06, 3.20393902061562e-11 }, { 6.92670982939798e-12, 6.28041373460333e-07, 0.00181716662265658, 0.164358087343011, 0.667648240537947, 0.164358087343011, 0.00181716662265658, 6.28041373460333e-07, 6.92670982939798e-12 }, { 1.33435250115072e-12, 2.68262287275912e-07, 0.00131540960035693, 0.156710265161694, 0.683948115360146, 0.156710265161694, 0.00131540960035693, 2.68262287275912e-07, 1.33435250115072e-12 }, { 2.26566373025895e-13, 1.07431039181398e-07, 0.000929953192043028, 0.148883337743699, 0.700373203665571, 0.148883337743699, 0.000929953192043028, 1.07431039181398e-07, 2.26566373025895e-13 }, { 3.34962614900577e-14, 4.00879573100650e-08, 0.000640669963524487, 0.140911957113860, 0.716894665771970, 0.140911957113860, 0.000640669963524487, 4.00879573100650e-08, 3.34962614900577e-14 }, { 4.22703128107637e-15, 1.38424445187243e-08, 0.000429046502624407, 0.132831202569309, 0.733479474194989, 0.132831202569309, 0.000429046502624407, 1.38424445187243e-08, 4.22703128107637e-15 }, { 4.44066354195823e-16, 4.38902520163207e-09, 0.000278524287048142, 0.124676435945350, 0.750090070762042, 0.124676435945350, 0.000278524287048142, 4.38902520163207e-09, 4.44066354195823e-16 }, { 6.28421948075850e-17, 1.26680667968016e-09, 0.000174723947382251, 0.116483255920966, 0.766684037730575, 0.116483255920966, 0.000174723947382251, 1.26680667968016e-09, 6.28421948075850e-17 }, { 0, 3.29608102079618e-10, 0.000105546210702447, 0.108287554166630, 0.783213798586258, 0.108287554166630, 0.000105546210702447, 3.29608102079618e-10, 0 }, { 0, 7.64617941663461e-11, 6.11514071086186e-05, 0.100125664014275, 0.799626369004327, 0.100125664014275, 6.11514071086186e-05, 7.64617941663461e-11, 0 }, { 0, 1.56183131947576e-11, 3.38289640416666e-05, 0.0920345789194055, 0.815863184201871, 0.0920345789194055, 3.38289640416666e-05, 1.56183131947576e-11, 0 }, { 0, 2.76961159394247e-12, 1.77774764499809e-05, 0.0840522047161711, 0.831860035609219, 0.0840522047161711, 1.77774764499809e-05, 2.76961159394247e-12, 0 }, { 0, 4.19556982824453e-13, 8.82316010031673e-06, 0.0762175981571222, 0.847547157364716, 0.0762175981571222, 8.82316010031673e-06, 4.19556982824453e-13, 0 }, { 0, 5.32909241187436e-14, 4.10833058012172e-06, 0.0685711359962434, 0.862849511346246, 0.0685711359962434, 4.10833058012172e-06, 5.32909241187436e-14, 0 }, { 0, 5.55112501021267e-15, 1.78109605436517e-06, 0.0611545550046143, 0.877687327798652, 0.0611545550046143, 1.78109605436517e-06, 5.55112501021267e-15, 0 }, { 0, 4.44089526338811e-16, 7.12668795255223e-07, 0.0540108042171262, 0.891976966228156, 0.0540108042171262, 7.12668795255223e-07, 4.44089526338811e-16, 0 }, { 0, 5.55111656942447e-17, 2.60541941965453e-07, 0.0471836561015230, 0.905632166713070, 0.0471836561015230, 2.60541941965453e-07, 5.55111656942447e-17, 0 }, { 0, 0, 8.60143288816673e-08, 0.0407170324794398, 0.918565763012463, 0.0407170324794398, 8.60143288816673e-08, 0, 0 }, { 0, 0, 2.52957787494346e-08, 0.0346540132902384, 0.930691922827966, 0.0346540132902384, 2.52957787494346e-08, 0, 0 }, { 0, 0, 6.52213977270222e-09, 0.0290355118639924, 0.941928963227736, 0.0290355118639924, 6.52213977270222e-09, 0, 0 }, { 0, 0, 1.44700523785346e-09, 0.0238986208211606, 0.952202755463668, 0.0238986208211606, 1.44700523785346e-09, 0, 0 }, { 0, 0, 2.70205746666363e-10, 0.0192746610558229, 0.961450677347943, 0.0192746610558229, 2.70205746666363e-10, 0, 0 }, { 0, 0, 4.13692413658850e-11, 0.0151870063104034, 0.969625987296455, 0.0151870063104034, 4.13692413658850e-11, 0, 0 }, { 0, 0, 5.03297403753322e-12, 0.0116488108003836, 0.976702378389167, 0.0116488108003836, 5.03297403753322e-12, 0, 0 }, { 0, 0, 4.68625138694279e-13, 0.00866083755617392, 0.982678324886715, 0.00866083755617392, 4.68625138694279e-13, 0, 0 } };

        // 3 tags
        private double[] probabilityOfObservation = new double[] { 0.150242, 0.257611, 0.555438, 0.2585, 0.151222 };
        private double[,] probability_observationGivenTrust = new double[,] { { 0.333, 0.3255, 0.332667, 0.335, 0.348 }, { 0.342, 0.3335, 0.335333, 0.337, 0.311 }, { 0.352, 0.3295, 0.332667, 0.3215, 0.348 }, { 0.333, 0.3215, 0.341667, 0.3285, 0.342 }, { 0.322, 0.3465, 0.332, 0.321, 0.347 }, { 0.322, 0.3355, 0.327667, 0.3445, 0.335 }, { 0.342, 0.3385, 0.334, 0.327, 0.325 }, { 0.343, 0.326, 0.332667, 0.334, 0.339 }, { 0.324, 0.336, 0.344667, 0.3185, 0.333 }, { 0.336, 0.3125, 0.358333, 0.328, 0.308 }, { 0.322, 0.3505, 0.313333, 0.3605, 0.316 }, { 0.33, 0.332, 0.340333, 0.34, 0.305 }, { 0.342, 0.332, 0.333667, 0.316, 0.361 }, { 0.308, 0.333, 0.343333, 0.3425, 0.311 }, { 0.332, 0.3405, 0.345333, 0.3285, 0.294 }, { 0.297, 0.344, 0.346333, 0.315, 0.346 }, { 0.297, 0.3315, 0.357, 0.321, 0.327 }, { 0.333, 0.3425, 0.333667, 0.333, 0.315 }, { 0.317, 0.3175, 0.349333, 0.3365, 0.327 }, { 0.295, 0.3515, 0.337333, 0.337, 0.316 }, { 0.306, 0.3525, 0.339333, 0.333, 0.305 }, { 0.311, 0.3375, 0.349667, 0.3285, 0.308 }, { 0.29, 0.343, 0.346, 0.3355, 0.315 }, { 0.317, 0.331, 0.349667, 0.3315, 0.309 }, { 0.317, 0.333, 0.336667, 0.3535, 0.3 }, { 0.315, 0.3195, 0.346333, 0.3525, 0.302 }, { 0.298, 0.3525, 0.359, 0.3155, 0.289 }, { 0.291, 0.323, 0.369, 0.3265, 0.303 }, { 0.28, 0.323, 0.366667, 0.338, 0.298 }, { 0.268, 0.3405, 0.358333, 0.3455, 0.285 }, { 0.278, 0.331, 0.363667, 0.342, 0.285 }, { 0.258, 0.345, 0.377333, 0.337, 0.246 }, { 0.279, 0.349, 0.350667, 0.336, 0.299 }, { 0.251, 0.3325, 0.378333, 0.3405, 0.268 }, { 0.259, 0.3385, 0.375333, 0.352, 0.234 }, { 0.243, 0.327, 0.386, 0.3405, 0.264 }, { 0.234, 0.3395, 0.391333, 0.3385, 0.236 }, { 0.221, 0.331, 0.413, 0.3275, 0.223 }, { 0.21, 0.3435, 0.387667, 0.3665, 0.207 }, { 0.22, 0.3385, 0.397, 0.3535, 0.205 }, { 0.218, 0.326, 0.423667, 0.317, 0.225 }, { 0.198, 0.34, 0.409667, 0.342, 0.209 }, { 0.195, 0.3195, 0.433667, 0.34, 0.185 }, { 0.204, 0.3245, 0.439, 0.3185, 0.193 }, { 0.179, 0.318, 0.439333, 0.353, 0.161 }, { 0.161, 0.338, 0.451, 0.325, 0.16 }, { 0.154, 0.333, 0.441667, 0.341, 0.173 }, { 0.15, 0.3395, 0.461333, 0.3235, 0.14 }, { 0.14, 0.3165, 0.469, 0.331, 0.158 }, { 0.135, 0.3325, 0.469, 0.334, 0.125 }, { 0.13, 0.324, 0.474, 0.336, 0.128 }, { 0.125, 0.3115, 0.489667, 0.333, 0.117 }, { 0.101, 0.328, 0.495, 0.313, 0.132 }, { 0.106, 0.3195, 0.504667, 0.3185, 0.104 }, { 0.093, 0.3205, 0.512, 0.3195, 0.091 }, { 0.101, 0.3225, 0.514667, 0.3085, 0.093 }, { 0.08, 0.303, 0.534, 0.3255, 0.061 }, { 0.065, 0.2955, 0.558333, 0.292, 0.085 }, { 0.05, 0.3045, 0.548667, 0.314, 0.067 }, { 0.058, 0.329, 0.535667, 0.3145, 0.048 }, { 0.044, 0.29, 0.574, 0.3005, 0.053 }, { 0.044, 0.2905, 0.574, 0.3105, 0.032 }, { 0.022, 0.293, 0.601, 0.278, 0.033 }, { 0.024, 0.287, 0.615667, 0.259, 0.037 }, { 0.021, 0.263, 0.641, 0.253, 0.024 }, { 0.022, 0.254, 0.642667, 0.26, 0.022 }, { 0.009, 0.263, 0.639667, 0.264, 0.018 }, { 0.019, 0.243, 0.659333, 0.2525, 0.012 }, { 0.012, 0.2355, 0.671333, 0.247, 0.009 }, { 0.01, 0.2275, 0.686, 0.235, 0.007 }, { 0.012, 0.2205, 0.688333, 0.238, 0.006 }, { 0.007, 0.2255, 0.707333, 0.207, 0.006 }, { 0.002, 0.188, 0.732333, 0.2095, 0.006 }, { 0.006, 0.19, 0.734333, 0.2025, 0.006 }, { 0.003, 0.1925, 0.742, 0.1915, 0.003 }, { 0.002, 0.1775, 0.758333, 0.1825, 0.003 }, { 0.001, 0.1665, 0.770333, 0.177, 0.001 }, { 0.002, 0.147, 0.781333, 0.1795, 0.001 }, { 1e-07, 0.156, 0.797, 0.148, 0.001 }, { 0.001, 0.141, 0.815, 0.136, 1e-07 }, { 1e-07, 0.137, 0.826333, 0.1235, 1e-07 }, { 1e-07, 0.1295, 0.83, 0.1255, 1e-07 }, { 1e-07, 0.1105, 0.851333, 0.1125, 1e-07 }, { 1e-07, 0.1095, 0.860333, 0.1, 1e-07 }, { 1e-07, 0.1025, 0.856333, 0.113, 1e-07 }, { 1e-07, 0.092, 0.879333, 0.0885, 0.001 }, { 1e-07, 0.0835, 0.896, 0.0725, 1e-07 }, { 1e-07, 0.076, 0.904667, 0.067, 1e-07 }, { 1e-07, 0.059, 0.920667, 0.06, 1e-07 }, { 1e-07, 0.0645, 0.923667, 0.05, 1e-07 }, { 1e-07, 0.0505, 0.936667, 0.0445, 1e-07 }, { 1e-07, 0.0425, 0.940333, 0.047, 1e-07 }, { 1e-07, 0.037, 0.952, 0.035, 1e-07 }, { 1e-07, 0.0265, 0.962, 0.0305, 1e-07 }, { 1e-07, 0.023, 0.973667, 0.0165, 1e-07 }, { 1e-07, 0.0135, 0.977667, 0.02, 1e-07 }, { 1e-07, 0.018, 0.974667, 0.02, 1e-07 }, { 1e-07, 0.015, 0.984333, 0.0085, 1e-07 }, { 1e-07, 0.006, 0.991, 0.0075, 1e-07 } };


        public BayesianProcessor():base()
        {
            
        }

        public int ProcessTags(List<UserTagReport> tags, List<User> Users, ExperimentOptions Options, int RealTag, bool isTraining)
        {
            int PredictedTag;
            if (isTraining)
            {
                PredictedTag = RealTag;
            }
            else
            {
                if (Options.maxTrust)
                {
                    PredictedTag = (from report in tags orderby Users[report.UserID].PredictedTrust descending, report.RandomColumnValue select report.Tag).ToArray()[0];
                }
                else
                {
                    double sum1 = 0, sum2 = 0;
                    int t;
                    foreach (var report in tags)
                    {
                        t = Users[report.UserID].PredictedTrust;
                        sum1 += t * report.Tag;
                        sum2 += t;
                    }
                    double a1 = (sum1 / sum2 - 1) / (DataSimulation.DefaultGroupSimulationOptions.TagOccupancies.Length - 1 - 1);
                    PredictedTag = policy.ConvertOccupancyValueToTag(a1, DataSimulation.DefaultGroupSimulationOptions.TagOccupancies);
                }
            }

            return PredictedTag;
        }

        public void UpdateTrustFromObservation(List<UserTagReport> tags, List<User> Users, int PredictedTag) // workes on updates
        {
            int selected_id, user_id;
            foreach (var report in tags)
            {
                selected_id = -1;
                user_id = report.UserID;
                double prior, posterior, update, maxLikelihood = -1;
                int k;
                double[] trustLikelihood = Users[user_id].Bayesian_TrustLikelihood;
                for (int t = 0; t < 99; t++)
                {
                    prior = trustLikelihood[t];
                    k = (int)(report.Tag - PredictedTag + param.maxTagOptions - 1);
                    update = probability_observationGivenTrust[t, k] / probabilityOfObservation[k];
                    posterior = prior * update;
                    trustLikelihood[t] = posterior;

                    if (posterior > maxLikelihood)
                    {
                        maxLikelihood = posterior;
                        selected_id = t;
                    }
                }
                Users[user_id].PredictedTrust = (int)(param.possibleTrusts[selected_id] * 100);
            }
        }
    }
}
