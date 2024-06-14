using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

using System;
using System.Collections;
using System.Collections.Generic;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine.UI;

using System.Threading;

public class EventManager : MonoBehaviour
{
    private Queue<Action> m_queueAction = new Queue<Action>();

    // public Transform head;
    // public Transform origin;
    // public Transform target;
    // public InputActionProperty recenterButton;

    JArray algorithm = JArray.Parse("[\r\n    {\r\n     \"No\": 1,\r\n     \"ID\": 1,\r\n     \"CurrentStepAlgorithm\": \"TEP (signs of life ?)\",\r\n     \"GuidingPadContent\": \"Info : Signs of life ? \\nButtons :  YES\\/NO\\n\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"Assess ABCs\",\r\n     \"NextSteps2\": \"1) Check pulse\\n2)\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 2,\r\n     \"ID\": 2,\r\n     \"CurrentStepAlgorithm\": \"Pulse ?\",\r\n     \"GuidingPadContent\": \"Info : Pulse ?\\nButtons : YES\\/NO\\n\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"Check pulse\",\r\n     \"NextSteps2\": \"1) Start CPR\\n2) Initiate BVM ventilation\\n3) Verify patient weight\"\r\n    },\r\n    {\r\n     \"No\": 3,\r\n     \"ID\": 3,\r\n     \"CurrentStepAlgorithm\": \"Start CPR\",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nButton : CPR started \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Verify patient weight\\n2) \\n3)\",\r\n     \"CurrentStep2\": \"1) Initiate CPR\\n2) Provide CPR Coaching\\n3) Initiate BVM ventilation\",\r\n     \"NextSteps2\": \"1) Verify patient weight\\n2) Check cardiac rhythm\\n3) Insert IV\\/IO\"\r\n    },\r\n    {\r\n     \"No\": 4,\r\n     \"ID\": 4,\r\n     \"CurrentStepAlgorithm\": \"Check for problem A and\\/or B\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Verify patient weight\\n2) \\n3)\",\r\n     \"CurrentStep2\": \"1) Assess suitability for ECMO\\n2)\\n3)\",\r\n     \"NextSteps2\": \"1) Activate ECMO team\\n2)\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 5,\r\n     \"CurrentStepAlgorithm\": \"First call to indicate weight or age\",\r\n     \"GuidingPadContent\": \"Info : Patient data (Optional)\\nAction : indicate weight and\\/or age and click on \\\"Confirm\\\" OR click on \\\"Skip\\\" to move forward\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Verify patient weight\\n2) \\n3)\",\r\n     \"NextSteps\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Verify patient weight\\n2) \\n3)\",\r\n     \"NextSteps2\": \"1) Check cardiac rhythm\\n2) Insert IV\\/IO\\n3) \"\r\n    },\r\n    {\r\n     \"No\": 6,\r\n     \"ID\": 5,\r\n     \"CurrentStepAlgorithm\": \"Check cardiac rythm \",\r\n     \"GuidingPadContent\": \"Info : Select rhythm\\nButtons : VF\\/pVT\\/Asystole\\/PEA    \\n\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Verify patient weight\\n2) \\n3)\",\r\n     \"NextSteps\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Check cardiac rhythm\\n2) Insert IV\\/IO\\n3) \",\r\n     \"NextSteps2\": \"1) Verbally prepare team for defibrillation\\n2) Prepare epinephrine doses x 3\\n3) \"\r\n    },\r\n    {\r\n     \"No\": 7,\r\n     \"CurrentStepAlgorithm\": \"Last call to indicate weight or age\",\r\n     \"GuidingPadContent\": \"Info : Indicate weight before defibrillation! OR Indicate weight before administering epinephrine!\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Verify patient weight\\n2) \\n3)\",\r\n     \"NextSteps\": \"1) Prepare epinephrine doses x 3\\n2) Prepare ketamine 1mg\\/kg x 2\\n3) \",\r\n     \"CurrentStep2\": \"1) Check cardiac rhythm\\n2) Insert IV\\/IO\\n3)\",\r\n     \"NextSteps2\": \"1) Verbally prepare team for defibrillation\\n2) Prepare epinephrine doses x 3\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 8,\r\n     \"ID\": 6,\r\n     \"CurrentStepAlgorithm\": \"Shock \",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nButton : Shock given \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2) Prepare ketamine 1mg\\/kg x 2\\n3)\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Verbally prepare team for defibrillation\\n2) Defibrillate 2J\\/kg \\n3) Resume CPR - Minimize pauses\",\r\n     \"NextSteps2\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 9,\r\n     \"ID\": 7,\r\n     \"CurrentStepAlgorithm\": \"CPR \",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nButton : CPR resumed \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2) Prepare ketamine 1mg\\/kg x 2\\n3)\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) Prepare rocuronium 1mg\\/kg \",\r\n     \"CurrentStep2\": \"1) Provide CPR\\n2) Provide CPR coaching\\n3) Prepare epinephrine doses x 3\",\r\n     \"NextSteps2\": \"1) Reassess airway\\n2) Reassess breathing\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 10,\r\n     \"ID\": 8,\r\n     \"CurrentStepAlgorithm\": \"Check for problem A and\\/or B\",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nToggle : a) airways PROBLEM\\/NORMAL\\nb) ventilation PROBLEM\\/NORMAL \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2) Prepare ketamine 1mg\\/kg x 2\\n3) Prepare rocuronium 1mg\\/kg\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Reassess airway\\n2) Reassess breathing\\n3)\",\r\n     \"NextSteps2\": \"1) \\n2)\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 11,\r\n     \"CurrentStepAlgorithm\": \"Problem A\",\r\n     \"GuidingPadContent\": \"Info : A- Obstructed airways\\nToggle : Repositioning\\/ Aspiration\\/ Foreign body extraction\\/ Wendel\\/ Guedel\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2) Prepare ketamine 1mg\\/kg x 2\\n3) Prepare rocuronium 1mg\\/kg\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Reassess airway\\n2) Reassess breathing\\n3)\",\r\n     \"NextSteps2\": \"1) Reposition airway as needed\\n2) Clear airway as needed\\n3) Improve ventilation as needed\"\r\n    },\r\n    {\r\n     \"No\": 12,\r\n     \"CurrentStepAlgorithm\": \"Problem B\",\r\n     \"GuidingPadContent\": \"Info : B - Non-intubated patient\\nToggle : Exsufflation - if asymmetric ventilation\\nOR Info : B - Intubated patient\\nToggle : Displacement\\/Tube obstruction\\/ Tension pneumothorax\\/ Equipment\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2) Prepare ketamine 1mg\\/kg x 2\\n3) Prepare rocuronium 1mg\\/kg\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Reassess airway\\n2) Reassess breathing\\n3)\",\r\n     \"NextSteps2\": \"1) Improve ventilation as needed\\n2) Check pulse\\n3) Check rhythm\"\r\n    },\r\n    {\r\n     \"No\": 13,\r\n     \"CurrentStepAlgorithm\": \"Waiting for the end of the 2 min timer\",\r\n     \"GuidingPadContent\": \"Info : Please wait until the two minutes are up before proceeding.\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Check pulse\\n2) Check rhythm\\n3)\",\r\n     \"NextSteps2\": \"1) \\n2) \\n3) \"\r\n    },\r\n    {\r\n     \"No\": 14,\r\n     \"ID\": 9,\r\n     \"CurrentStepAlgorithm\": \"Check Pulse\",\r\n     \"GuidingPadContent\": \"Info : Pulse ?\\nButtons : YES\\/NO\\n\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare ketamine 1mg\\/kg\\n2) Prepare rocuronium 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Check pulse\\n2) Check rhythm\\n3)\",\r\n     \"NextSteps2\": \"1) Verbally prepare team for defibrillation\\n2) Prepare epinephrine doses x 3\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 15,\r\n     \"ID\": 10,\r\n     \"CurrentStepAlgorithm\": \"Check cardiac rythm\",\r\n     \"GuidingPadContent\": \"Info : Select rhythm\\nButtons : VF\\/pVT\\/Asystole\\/PEA    \\n\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare ketamine 1mg\\/kg\\n2) Prepare rocuronium 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Check pulse\\n2) Check rhythm\\n3)\",\r\n     \"NextSteps2\": \"1) Verbally prepare team for defibrillation\\n2) Prepare epinephrine doses x 3\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 16,\r\n     \"ID\": 11,\r\n     \"CurrentStepAlgorithm\": \"Shock \",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nButton : Shock given \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2) Prepare ketamine 1mg\\/kg x 2\\n3) Prepare rocuronium 1mg\\/kg\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Verbally prepare team for defibrillation\\n2) Defibrillate 4J\\/kg \\n3) Resume CPR - Minimize pauses\",\r\n     \"NextSteps2\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 17,\r\n     \"ID\": 12,\r\n     \"CurrentStepAlgorithm\": \"CPR + Epinephrine \",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nButton : CPR resumed \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Provide CPR\\n2) Provide CPR Coaching\\n3) Epinephrine 0.1 ml\\/kg  IV\\/IO\",\r\n     \"NextSteps2\": \"1) Prepare for intubation \\/ LMA insertion\\n2)\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 18,\r\n     \"ID\": 13,\r\n     \"CurrentStepAlgorithm\": \"Check for problem A and\\/or B\",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nToggle : a) airways PROBLEM\\/NORMAL\\nb) ventilation PROBLEM\\/NORMAL \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Intubate or insert LMA\\n2)\\n3)\",\r\n     \"NextSteps2\": \"1) Check tube placement\\n2)\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 19,\r\n     \"CurrentStepAlgorithm\": \"Problem A\",\r\n     \"GuidingPadContent\": \"Info : A- Obstructed airways\\nToggle : Repositioning\\/ Aspiration\\/ Foreign body extraction\\/ Wendel\\/ Guedel\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Check tube \\/ LMA placement\\n2) Reassess airway\\n3) Clear \\/ suction tube as needed\",\r\n     \"NextSteps2\": \"1) Reintubate \\/ reinsert LMA as needed\\n2) \\n3)\"\r\n    },\r\n    {\r\n     \"No\": 20,\r\n     \"CurrentStepAlgorithm\": \"Problem B\",\r\n     \"GuidingPadContent\": \"Info : B - Non-intubated patient\\nToggle : Exsufflation - if asymmetric ventilation\\nOR Info : B - Intubated patient\\nToggle : Displacement\\/Tube obstruction\\/ Tension pneumothorax\\/ Equipment\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Check tube \\/ LMA placement\\n2) Reassess airway\\n3) Clear \\/ suction tube as needed\",\r\n     \"NextSteps2\": \"1) Reintubate \\/ reinsert LMA as needed\\n2) Check pulse\\n3) Check rhythm\"\r\n    },\r\n    {\r\n     \"No\": 21,\r\n     \"ID\": 14,\r\n     \"CurrentStepAlgorithm\": \"Check Pulse\",\r\n     \"GuidingPadContent\": \"Info : Pulse ?\\nButtons : YES\\/NO\\n\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare ketamine 1mg\\/kg\\n2) Prepare rocuronium 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Check pulse\\n2) Check rhythm\\n3)\",\r\n     \"NextSteps2\": \"1) Verbally prepare team for defibrillation\\n2) Prepare epinephrine doses x 3\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 22,\r\n     \"ID\": 15,\r\n     \"CurrentStepAlgorithm\": \"Check cardiac rythm \",\r\n     \"GuidingPadContent\": \"Info : Select rhythm\\nButtons : VF\\/pVT\\/Asystole\\/PEA    \\n\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare ketamine 1mg\\/kg\\n2) Prepare rocuronium 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Check pulse\\n2) Check rhythm\\n3\",\r\n     \"NextSteps2\": \"1) Verbally prepare team for defibrillation\\n2) Prepare epinephrine doses x 3\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 23,\r\n     \"ID\": 16,\r\n     \"CurrentStepAlgorithm\": \"Shock \",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nButton : Shock given \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2) Prepare ketamine 1mg\\/kg x 2\\n3) Prepare rocuronium 1mg\\/kg\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Verbally prepare team for defibrillation\\n2) Defibrillate 4J\\/kg \\n3) Resume CPR - Minimize pauses\",\r\n     \"NextSteps2\": \"1) Prepare amiodarone and\\/or lidocaine\\n2) Consider reversible causes\\n3) \"\r\n    },\r\n    {\r\n     \"No\": 24,\r\n     \"ID\": 17,\r\n     \"CurrentStepAlgorithm\": \"CPR \",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nButton : CPR resumed \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Provide CPR\\n2) Provide CPR Coaching\\n3) \",\r\n     \"NextSteps2\": \"1) Reassess airway\\n2) Reassess breathing \\/ ventilation\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 25,\r\n     \"ID\": 18,\r\n     \"CurrentStepAlgorithm\": \"Check for problem A and\\/or B\",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nToggle : a) airways PROBLEM\\/NORMAL\\nb) ventilation PROBLEM\\/NORMAL \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Reassess airway\\n2) Reassess breathing \\/ ventilation\\n3)\",\r\n     \"NextSteps2\": \"1) Clear aiway as needed\\n2) Improve ventilation as needed\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 26,\r\n     \"CurrentStepAlgorithm\": \"Problem A\",\r\n     \"GuidingPadContent\": \"Info : A- Obstructed airways\\nToggle : Repositioning\\/ Aspiration\\/ Foreign body extraction\\/ Wendel\\/ Guedel\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Check tube \\/ LMA placement\\n2) Reassess airway\\n3) Clear \\/ suction tube as needed\",\r\n     \"NextSteps2\": \"1) Reintubate \\/ reinsert LMA as needed\\n2) \\n3)\"\r\n    },\r\n    {\r\n     \"No\": 27,\r\n     \"CurrentStepAlgorithm\": \"Problem B\",\r\n     \"GuidingPadContent\": \"Info : B - Non-intubated patient\\nToggle : Exsufflation - if asymmetric ventilation\\nOR Info : B - Intubated patient\\nToggle : Displacement\\/Tube obstruction\\/ Tension pneumothorax\\/ Equipment\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Check tube \\/ LMA placement\\n2) Reassess airway\\n3) Clear \\/ suction tube as needed\",\r\n     \"NextSteps2\": \"1) Reintubate \\/ reinsert LMA as needed\\n2) Check pulse\\n3) Check rhythm\"\r\n    },\r\n    {\r\n     \"No\": 28,\r\n     \"ID\": 19,\r\n     \"CurrentStepAlgorithm\": \"Check Pulse\",\r\n     \"GuidingPadContent\": \"Info : Pulse ?\\nButtons : YES\\/NO\\n\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Check pulse\\n2) Check rhythm\\n3)\",\r\n     \"NextSteps2\": \"1) Verbally prepare team for defibrillation\\n2) Prepare epinephrine doses x 3\\n3) Prepare amiodarone and\\/or lidocaine\"\r\n    },\r\n    {\r\n     \"No\": 29,\r\n     \"ID\": 20,\r\n     \"CurrentStepAlgorithm\": \"Check cardiac rythm \",\r\n     \"GuidingPadContent\": \"Info : Select rhythm\\nButtons : VF\\/pVT\\/Asystole\\/PEA    \\n\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Check pulse\\n2) Check rhythm\\n3)\",\r\n     \"NextSteps2\": \"1) Verbally prepare team for defibrillation\\n2) Prepare epinephrine doses x 3\\n3) Prepare amiodarone and\\/or lidocaine\"\r\n    },\r\n    {\r\n     \"No\": 30,\r\n     \"ID\": 21,\r\n     \"CurrentStepAlgorithm\": \"CPR + Epinephrine \",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nButton : CPR resumed \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Provide CPR\\n2) Provide CPR Coaching\\n3) Epinephrine 0.1 ml\\/kg  IV\\/IO\",\r\n     \"NextSteps2\": \"1) Prepare amiodarone and\\/or lidocaine\\n2)\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 31,\r\n     \"ID\": 22,\r\n     \"CurrentStepAlgorithm\": \"Check for problem A and\\/or B\",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nToggle : a) airways PROBLEM\\/NORMAL\\nb) ventilation PROBLEM\\/NORMAL \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Reassess airway\\n2) Reassess breathing \\/ ventilation\\n3)\",\r\n     \"NextSteps2\": \"1) Clear aiway as needed\\n2) Improve ventilation as needed\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 32,\r\n     \"CurrentStepAlgorithm\": \"Problem A\",\r\n     \"GuidingPadContent\": \"Info : A- Obstructed airways\\nToggle : Repositioning\\/ Aspiration\\/ Foreign body extraction\\/ Wendel\\/ Guedel\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Check tube \\/ LMA placement\\n2) Reassess airway\\n3) Clear \\/ suction tube as needed\",\r\n     \"NextSteps2\": \"1) Reintubate \\/ reinsert LMA as needed\\n2) \\n3)\"\r\n    },\r\n    {\r\n     \"No\": 33,\r\n     \"CurrentStepAlgorithm\": \"Problem B\",\r\n     \"GuidingPadContent\": \"Info : B - Non-intubated patient\\nToggle : Exsufflation - if asymmetric ventilation\\nOR Info : B - Intubated patient\\nToggle : Displacement\\/Tube obstruction\\/ Tension pneumothorax\\/ Equipment\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Check tube \\/ LMA placement\\n2) Reassess airway\\n3) Clear \\/ suction tube as needed\",\r\n     \"NextSteps2\": \"1) Reintubate \\/ reinsert LMA as needed\\n2) Check pulse\\n3) Check rhythm\"\r\n    },\r\n    {\r\n     \"No\": 34,\r\n     \"ID\": 23,\r\n     \"CurrentStepAlgorithm\": \"Check Pulse\",\r\n     \"GuidingPadContent\": \"Info : Pulse ?\\nButtons : YES\\/NO\\n\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Check pulse\\n2) Check rhythm\\n3)\",\r\n     \"NextSteps2\": \"1) Verbally prepare team for defibrillation\\n2) Prepare epinephrine doses x 3\\n3) Prepare amiodarone and\\/or lidocaine\"\r\n    },\r\n    {\r\n     \"No\": 35,\r\n     \"ID\": 24,\r\n     \"CurrentStepAlgorithm\": \"Check cardiac rythm \",\r\n     \"GuidingPadContent\": \"Info : Select rhythm\\nButtons : VF\\/pVT\\/Asystole\\/PEA    \\n\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Check pulse\\n2) Check rhythm\\n3)\",\r\n     \"NextSteps2\": \"1) Verbally prepare team for defibrillation\\n2) Prepare epinephrine doses x 3\\n3) Prepare amiodarone and\\/or lidocaine\"\r\n    },\r\n    {\r\n     \"No\": 36,\r\n     \"ID\": 25,\r\n     \"CurrentStepAlgorithm\": \"CPR + Amiodarone or Lidocaine\",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nButton : CPR resumed \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Provide CPR\\n2) Provide CPR Coaching\\n3) Amiodarone or Lidocaine IV\\/IO\",\r\n     \"NextSteps2\": \"1) Consider reversible causes\\n2) Prepare epinephrine doses x 3\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 37,\r\n     \"ID\": 26,\r\n     \"CurrentStepAlgorithm\": \"Check for problem A and\\/or B\",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nToggle : a) airways PROBLEM\\/NORMAL\\nb) ventilation PROBLEM\\/NORMAL \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Reassess airway\\n2) Reassess breathing \\/ ventilation\\n3)\",\r\n     \"NextSteps2\": \"1) Clear aiway as needed\\n2) Improve ventilation as needed\\n3)\"\r\n    },\r\n    {\r\n     \"No\": 38,\r\n     \"CurrentStepAlgorithm\": \"Problem A\",\r\n     \"GuidingPadContent\": \"Info : A- Obstructed airways\\nToggle : Repositioning\\/ Aspiration\\/ Foreign body extraction\\/ Wendel\\/ Guedel\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Check tube \\/ LMA placement\\n2) Reassess airway\\n3) Clear \\/ suction tube as needed\",\r\n     \"NextSteps2\": \"1) Reintubate \\/ reinsert LMA as needed\\n2) \\n3)\"\r\n    },\r\n    {\r\n     \"No\": 39,\r\n     \"CurrentStepAlgorithm\": \"Problem B\",\r\n     \"GuidingPadContent\": \"Info : B - Non-intubated patient\\nToggle : Exsufflation - if asymmetric ventilation\\nOR Info : B - Intubated patient\\nToggle : Displacement\\/Tube obstruction\\/ Tension pneumothorax\\/ Equipment\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Check tube \\/ LMA placement\\n2) Reassess airway\\n3) Clear \\/ suction tube as needed\",\r\n     \"NextSteps2\": \"1) Reintubate \\/ reinsert LMA as needed\\n2) Check pulse\\n3) Check rhythm\"\r\n    },\r\n    {\r\n     \"No\": 40,\r\n     \"ID\": 27,\r\n     \"CurrentStepAlgorithm\": \"Check Pulse\",\r\n     \"GuidingPadContent\": \"Info : Pulse ?\\nButtons : YES\\/NO\\n\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Check pulse\\n2) Check rhythm\\n3)\",\r\n     \"NextSteps2\": \"1) Verbally prepare team for defibrillation\\n2) Prepare epinephrine doses x 3\\n3) Consider reversible causes\"\r\n    },\r\n    {\r\n     \"No\": 41,\r\n     \"ID\": 28,\r\n     \"CurrentStepAlgorithm\": \"Check cardiac rythm \",\r\n     \"GuidingPadContent\": \"Info : Select rhythm\\nButtons : VF\\/pVT\\/Asystole\\/PEA    \\n\",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) Prepare amiodarone 5mg\\/kg\\n2) Prepare lidocaine 1mg\\/kg\\n3) \",\r\n     \"CurrentStep2\": \"1) Check pulse\\n2) Check rhythm\\n3)\",\r\n     \"NextSteps2\": \"1) Verbally prepare team for defibrillation\\n2) Prepare epinephrine doses x 3\\n3) Prepare amiodarone and\\/or lidocaine\"\r\n    },\r\n    {\r\n     \"No\": 42,\r\n     \"ID\": 29,\r\n     \"CurrentStepAlgorithm\": \"CPR + Epinephrine\",\r\n     \"GuidingPadContent\": \"Info : Confirm this action\\nButton : CPR resumed \",\r\n     \"TeamScreen\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep\": \"1) Prepare epinephrine doses x 3\\n2)\\n3)\",\r\n     \"NextSteps\": \"1) \\n2)\\n3)\",\r\n     \"CurrentStep2\": \"1) Provide CPR\\n2) Provide CPR Coaching\\n3) Epinephrine 0.1 ml\\/kg  IV\\/IO\",\r\n     \"NextSteps2\": \"1) Consider reversible causes \\n2)\\n3)\"\r\n    }\r\n   ]");

    TextMeshProUGUI timer1;
    TextMeshProUGUI timer2;

    TextMeshProUGUI Doc_Cur_1;
    TextMeshProUGUI Doc_Cur_2;
    TextMeshProUGUI Doc_Cur_3;
    TextMeshProUGUI Doc_Next_1;
    TextMeshProUGUI Doc_Next_2;
    TextMeshProUGUI Doc_Next_3;
    TextMeshProUGUI Nurse_Cur_1;
    TextMeshProUGUI Nurse_Cur_2;
    TextMeshProUGUI Nurse_Cur_3;
    TextMeshProUGUI Nurse_Next_1;
    TextMeshProUGUI Nurse_Next_2;
    TextMeshProUGUI Nurse_Next_3;
    RawImage AlgoImg;

    double time1 = 0;
    double time2 = 0;

    // Update is called once per frame

    public SocketIOUnity socket;
    int idx = 1;
    double startTimestamp = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(algorithm);
        
        if (GameObject.FindWithTag("CPRTimer") != null) {
            timer1 = GameObject.FindWithTag("CPRTimer").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("EpiTimer") != null) {
           timer2 = GameObject.FindWithTag("EpiTimer").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("Doc_Cur_1") != null) {
           Doc_Cur_1 = GameObject.FindWithTag("Doc_Cur_1").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("Doc_Cur_2") != null) {
           Doc_Cur_2 = GameObject.FindWithTag("Doc_Cur_2").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("Doc_Cur_3") != null) {
           Doc_Cur_3 = GameObject.FindWithTag("Doc_Cur_3").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("Doc_Next_1") != null) {
           Doc_Next_1 = GameObject.FindWithTag("Doc_Next_1").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("Doc_Next_2") != null) {
           Doc_Next_2 = GameObject.FindWithTag("Doc_Next_2").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("Doc_Next_3") != null) {
           Doc_Next_3 = GameObject.FindWithTag("Doc_Next_3").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("Nurse_Cur_1") != null) {
           Nurse_Cur_1 = GameObject.FindWithTag("Nurse_Cur_1").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("Nurse_Cur_2") != null) {
           Nurse_Cur_2 = GameObject.FindWithTag("Nurse_Cur_2").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("Nurse_Cur_3") != null) {
           Nurse_Cur_3 = GameObject.FindWithTag("Nurse_Cur_3").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("Nurse_Next_1") != null) {
           Nurse_Next_1 = GameObject.FindWithTag("Nurse_Next_1").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("Nurse_Next_2") != null) {
           Nurse_Next_2 = GameObject.FindWithTag("Nurse_Next_2").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("Nurse_Next_3") != null) {
           Nurse_Next_3 = GameObject.FindWithTag("Nurse_Next_3").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("algoImg") != null) {
           AlgoImg = GameObject.FindWithTag("algoImg").GetComponent<RawImage>();
        }
        //Connect to the server for real-time communication.
        //We don't need this connection in the future.
        var uri = new Uri("http://136.159.140.66");

        socket = new SocketIOUnity(uri);

        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("socket.OnConnected");
        };
        //Ready to listen whenever receiving data from the server
        /*
        TO-DO we will chagne this part.
        Now we are receiving data from server,
        We will invoke event from the local script
        ex) 
            setTimeout(()=> evoke_event(idx), 5000)
            setTimeout(()=> evoke_event(1), 5000)
            setTimeout(()=> evoke_event(3), 5000)
            setTimeout(()=> evoke_event(5), 5000)
        */
        socket.On("currentStatus", (response) => {
            Debug.Log("currentStatus");
            Debug.Log(response);

            idx = response.GetValue<int>();
            Debug.Log("idx: " + idx);

            DateTime currentTime = DateTime.UtcNow;
            long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();

            startTimestamp = response.GetValue<double>(1);
            Debug.Log("startTimestamp:" + startTimestamp);

            if (unixTime * 1000 - startTimestamp < 120) {

            }

            time1 = (startTimestamp - unixTime * 1000) / 1000 + 120;
            time2 = (startTimestamp - unixTime * 1000) / 1000 + 300;

            //UI update
            //Due to this is not the main process(?), we need to update UI like the below
            m_queueAction.Enqueue(() => UpdateUI(idx));
            /*
                TO-DO We need to add some Visual, Audio, and Haptic notification here by calling the below function.
                Add_Notifications();
            */

        });

        Debug.Log("Connecting...");
        socket.Connect();
    }

    void Add_Notifications()
    {
        /* 
        TO-DO
        Use switch or if statements
        case 1: Visual (Arrow to spatial point(x2, y2, z2) from users current spatial point(x1, y1, z1))
            Start with making ball at the point(x2, y2, z2) and point(x1, y1, z1)
            making ball id done by UI, or Script
            1)In case of UI
                making balls in advanced by using UI.
                we can simply turn off the object that we do not need at specific time,
                and turn on the object that we need at that time.
            2)In case of Script
                we can add what we need, and remove what we do not need at specific time.
        === 
        case 2: Audio (spatial audio)
            Please take a look at https://learn.microsoft.com/en-us/training/modules/spatial-audio-tutorials-mrtk/
        ===
        case 3: Haptic (based on the spatial audio)
            **Need to find is it possible to use without sound even using spatial audio?
        */
    }

    //UI update
    void UpdateUI(int idx)
    {
        Debug.Log("UpdateUI");
        Debug.Log("UpdateUI: " + idx);
        // TextMeshProUGUI temp = GameObject.FindWithTag("Amiodarone").GetComponent<TextMeshProUGUI>();
        var lastID = 1;
        foreach(JObject obj in algorithm)
        {
            try
            {
                if (obj["ID"] != null) {
                    lastID = int.Parse(obj["ID"].ToString());
                }
                if (int.Parse(obj["No"].ToString()) == idx) {

                    if (AlgoImg != null)
                    {
                        if (obj["ID"] != null) {
                            var texture = Resources.Load<Texture>("Texture/" + obj["ID"]);
                            AlgoImg.texture = texture;
                        } else {
                            var texture = Resources.Load<Texture>("Texture/" + lastID);
                            AlgoImg.texture = texture;
                        }
                    }

                    Debug.Log(obj);
                    Init_Tasks();
                    if (obj["CurrentStep"] == null) {
                        
                    } else {
                        string[] instrunctions = obj["CurrentStep"].ToString().Split('\n');

                        for(int i = 0; i < instrunctions.Length; i++)
                        {
                            string instruction = instrunctions[i];

                            if (i == 0) {
                                if (Nurse_Cur_1 != null) {
                                    Nurse_Cur_1.text = instruction;
                                }
                            } else if (i == 1) {
                                if (Nurse_Cur_2 != null) {
                                    Nurse_Cur_2.text = instruction;
                                }
                            } else if (i == 2) {
                                if (Nurse_Cur_3 != null) {
                                    Nurse_Cur_3.text = instruction;
                                }
                            }
                        }
                    }

                    if (obj["NextSteps"] == null) {
                        
                    } else {
                        string[] instrunctions = obj["NextSteps"].ToString().Split('\n');

                        for(int i = 0; i < instrunctions.Length; i++)
                        {
                            string instruction = instrunctions[i];
                            if (i == 0) {
                                if (Nurse_Next_1 != null) {
                                    Nurse_Next_1.text = instruction;
                                }
                            } else if (i == 1) {
                                if (Nurse_Next_2 != null) {
                                    Nurse_Next_2.text = instruction;
                                }
                            } else if (i == 2) {
                                if (Nurse_Next_3 != null) {
                                    Nurse_Next_3.text = instruction;
                                }
                            }
                        }
                    }

                    if (obj["CurrentStep2"] == null) {
                        
                    } else {
                        string[] instrunctions = obj["CurrentStep2"].ToString().Split('\n');

                        for(int i = 0; i < instrunctions.Length; i++)
                        {
                            string instruction = instrunctions[i];

                            if (i == 0) {
                                if (Doc_Cur_1 != null) {
                                    Doc_Cur_1.text = instruction;
                                }
                            } else if (i == 1) {
                                if (Doc_Cur_2 != null) {
                                    Doc_Cur_2.text = instruction;
                                }
                            } else if (i == 2) {
                                if (Doc_Cur_3 != null) {
                                    Doc_Cur_3.text = instruction;
                                }
                            }
                        }
                    }

                    if (obj["NextSteps2"] == null) {
                        
                    } else {
                        string[] instrunctions = obj["NextSteps2"].ToString().Split('\n');

                        for(int i = 0; i < instrunctions.Length; i++)
                        {
                            string instruction = instrunctions[i];
                            if (i == 0) {
                                if (Doc_Next_1 != null) {
                                    Doc_Next_1.text = instruction;
                                }
                            } else if (i == 1) {
                                if (Doc_Next_2 != null) {
                                    Doc_Next_2.text = instruction;
                                }
                            } else if (i == 2) {
                                if (Doc_Next_3 != null) {
                                    Doc_Next_3.text = instruction;
                                }
                            }
                        }
                    }
                }
            } catch (Exception e) {
                Debug.Log(e);
            }

        }
    }

    public void Init_Tasks()
    {
        if (Doc_Cur_1 != null) {
            Doc_Cur_1.text = "";
        }
        if (Doc_Cur_2 != null) {
            Doc_Cur_2.text = "";
        }
        if (Doc_Cur_3 != null) {
            Doc_Cur_3.text = "";
        }
        if (Doc_Next_1 != null) {
            Doc_Next_1.text = "";
        }
        if (Doc_Next_2 != null) {
            Doc_Next_2.text = "";
        }
        if (Doc_Next_3 != null) {
            Doc_Next_3.text = "";
        }
        if (Nurse_Cur_1 != null) {
            Nurse_Cur_1.text = "";
        }
        if (Nurse_Cur_2 != null) {
            Nurse_Cur_2.text = "";
        }
        if (Nurse_Cur_3 != null) {
            Nurse_Cur_3.text = "";
        }
        if (Nurse_Next_1 != null) {
            Nurse_Next_1.text = "";
        }
        if (Nurse_Next_2 != null) {
            Nurse_Next_2.text = "";
        }
        if (Nurse_Next_3 != null) {
            Nurse_Next_3.text = "";
        }
    }

    public void UpdateClock()
    {
        if (timer1 != null && time1 > 0) {
            time1 -= Time.deltaTime;
            string min = ((int)time1 / 60 % 60 ).ToString();
            if (min.Length == 1) {
                min = "0" + min;
            }
            string sec = ((int)time1 % 60 ).ToString();
            if (sec.Length == 1) {
                sec = "0" + sec;
            }
            timer1.text = min + ":" + sec;
        }

        if (timer2 != null && time2 > 0) {
            time2 -= Time.deltaTime;
            string min = ((int)time2 / 60 % 60 ).ToString();
            if (min.Length == 1) {
                min = "0" + min;
            }
            string sec = ((int)time2 % 60 ).ToString();
            if (sec.Length == 1) {
                sec = "0" + sec;
            }
            timer2.text = min + ":" + sec;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateClock();
        while (m_queueAction.Count > 0)
        {
            m_queueAction.Dequeue().Invoke();
        }
    }

    public void ToMain()
    { 
        SceneManager.LoadScene("main_scene");
    }

    public void Doctor1()
    { 
        SceneManager.LoadScene("hmd_doctor_1");
    }

    public void Doctor2()
    { 
        SceneManager.LoadScene("hmd_doctor_2");
    }

    public void Doctor3()
    { 
        SceneManager.LoadScene("hmd_doctor_3");
    }

    public void Doctor4()
    { 
        SceneManager.LoadScene("hmd_doctor_4");
    }

    public void Doctor5()
    { 
        SceneManager.LoadScene("hmd_doctor_5");
    }

    public void Doctor6()
    { 
        SceneManager.LoadScene("hmd_doctor_6");
    }

    public void Doctor7()
    { 
        SceneManager.LoadScene("hmd_doctor_7");
    }

    public void Doctor8()
    { 
        SceneManager.LoadScene("hmd_doctor_8");
    }

    public void Doctor9()
    { 
        SceneManager.LoadScene("hmd_doctor_9");
    }


    public void Nurse1()
    { 
        SceneManager.LoadScene("hmd_nurse_1");
    }

    public void Nurse2()
    { 
        SceneManager.LoadScene("hmd_nurse_2");
    }

    public void Nurse3()
    { 
        SceneManager.LoadScene("hmd_nurse_3");
    }

    public void Nurse4()
    { 
        SceneManager.LoadScene("hmd_nurse_4");
    }

    public void Nurse5()
    { 
        SceneManager.LoadScene("hmd_nurse_5");
    }

    // public void ResetCenter()
    // { 
    //     Vector3 offset = head.position - origin.position;
    //     offset.y = 0;
    //     origin.position = target.position - offset;

    //     Vector3 targetForward = target.forward;
    //     targetForward.y = 0;
    //     Vector3 cameraForward = head.forward;
    //     cameraForward.y = 0;

    //     float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);

    //     origin.RotateAround(head.position, Vector3.up, angle);
    // }
}
