# 워크 스탠다드

- 이 문서의 깃허브 링크 :
https://github.com/KorStrix/StrixLibrary/tree/master/99.Docs/UnityWorkStandard
-
- 제가 작업한 작업물들은 이 문서에 있는 워크 스탠다드를 따라 작업하였습니다.
- 워크 스탠다드는 제가 경험하면서 효율적인것은 따르고, 따르다가 비효율적인 것은 버리기도 하였습니다.


---
## 작업 스타일

- 폴더부터 코드 네이밍, 코드 변수 및 함수 정의 위치, 유니티 오브젝트 네이밍 및 하이어라키까지 정하는 편입니다.
  - 이에 대해 항상 부가 설명을 하자면, 프로그래밍은 개발보다 유지보수 비용이 훨씬 크다고 생각합니다.
  - 또한, **담당자에게 물어보지 않고 스스로 예측 가능** 하는 것을 최고로 생각 하고 있습니다.
    - 부가 설명을 더 하자면, 현실의 방의 예를 듭니다. 아무리 어두운 방이라도, 우리는 항상 문고리가 어느 위치쯤에 있는지, 전등 스위치가 문 주변의 벽에 있다는 점을 누가 알려주지 않아도 **스스로 예측** 할 수 있습니다.
  - 이런식으로 네이밍을 하면, 어느 클래스의 이름만 보아도 어느 계층의 어느 패턴을 상속받은 클래스라는 점, 어느 함수의 이름만 보아도 어느 위치에 있는 지를 **스스로 예측** 할 수 있습니다.

- ``깨진 창문``을 주의하기 위하여 클린코드의 ``나중은 오지 않는다``(르블랑의 법칙)와, ``왔을 때보다 좀 더 깔끔하게``보이 스카웃 법칙을 따르도록 노력하는 편입니다.
 - 이에 관련된 SlideShare [링크](https://www.slideshare.net/jrogue/ss-38012889)

---
![](https://postfiles.pstatic.net/MjAxODA1MDdfNjAg/MDAxNTI1NjgyMTk1OTIw.KWb2jh-HlZxIpGvqURDhOToGnoJTmN0PCbjZLzgQUm0g.4hdmcELocTlAEAKNhnFRIvQpRDbemTH05Nik6nNYlYMg.JPEG.strix13/bandicam_2018-05-07_17-36-23-518.jpg?type=w773)

ㄴ 제가 작성하고 사용하는 Visual Studio - Unity Item Template 입니다. [깃허브 링크](https://github.com/KorStrix/StrixLibrary/tree/master/99.CoreAsset/VSItemTemplate)


---
### 목차
1. 유니티 공용
2. 유니티 - 프로젝트 뷰
3. 유니티 - 하이어라키 뷰
4. 스크립팅
5. 스크립팅 - 클래스
6. 스크립팅 - 변수
7. 스크립팅 - 함수

---
## 1. 유니티 공용

**1-1. 이름에는 `한글 및 영어, 숫자`, "` `"(공백), "`_`" 등을 제외한 문자는 가급적이면 넣지 않습니다.**
- 코드에서 바로 불러오기 위함입니다.

**1-2. 자주 사용하는 것은 상단에 배치합니다.**

- 프로젝트 뷰의 경우 폴더 앞에 숫자를 사용하여 폴더를 상단에 배치할 수 있습니다.
- 하이어라키 뷰의 경우 자식 순서나 컴포넌트의 순서를 상단에 배치할 수 있습니다.

**1-3. 한 공간 안에 5개 이상의 요소가 있을 경우, 되도록이면 카테고리를 하나 만들어 분류합니다.**

- 프로젝트 뷰의 경우 카테고리 성격의 폴더를 생성합니다.
- 하이어라키 뷰의 경우 카테고리 성격의 Empty Game Object를 생성합니다.

---
## 2. 유니티 - 프로젝트 뷰

이 항목은 Asset 폴더 내의 내부 폴더, 파일 네이밍 및 배치에 대한 규칙입니다.

**2-1. 스크립트는 한 폴더 내에 있어야 합니다.**

  - 스크립트가 작업자마다 각각 다른 부분에 배치할 경우, 스크립트의 사용/미사용 유무를 파악하기가 힘들기 때문입니다.

**2-2. 에셋 스토어에서 구매한 에셋의 경우, 가급적이면 에셋 만 들어있는 폴더 내에 넣어주시기 바랍니다.**

  - 제 경우는 ExternalAsset, ExternalAsset_Resource 등으로 구분하였습니다.
  - ExternalAsset와 ExternalAsset_Resource 폴더의 구분 기준은 비 프로그래머가 해당 에셋을 바로 사용할 수 있는지 유무입니다.
    - Ex ) 이미지, 사운드 등

**2-3. Resources 폴더 내에는 가급적이면 Resources 함수를 사용하는 오브젝트만 넣어주시기 바랍니다.**

  - Resources에 들어가는 리소스는 빌드에 포함하여 빌드 용량이 많아집니다.

**2-4. Asset 폴더의 최상단에는 되도록이면 파일이 없어야 합니다. 가급적이면 카테고리 성격의 폴더를 만들어 주시기 바랍니다.**

**2-5. Scene 파일의 경우 되도록이면 Asset폴더의 최상단에 배치해주시기 바랍니다.**

  - Scene의 경우 가장 번번히 여는 파일이기 때문에 폴더 내부 깊게 있으면 번거롭기 때문입니다.

---
## 3. 유니티 - 하이어라키 뷰

이 항목은 게임 내의 오브젝트 네이밍 및 부모자식 관계도에 대한 규칙입니다.

**3-1. 게임 오브젝트 이름은 가급적이면 해당 오브젝트의 대표격인 컴포넌트를 접두어로 명명합니다.**

**3-2. UI의 경우 게임 오브젝트 이름은 UI Element 성격을 접두어로 붙입니다.**

  - 하이어라키상에서 오브젝트를 일일이 누르지 않고 한눈에 구조를 파악하기 위함입니다.

```
- UIButton의 경우 : Button_Somthing
- UIImage의 경우  : Image_Somthing
```

**3-3. 컴포넌트 참조는 가급적이면 하이어라키 구조를 이용합니다. 게임 오브젝트 역시 이에 맞게 배치합니다.**

  - 하이어라키 구조를 이용하지 않을 경우, 어떠한 오브젝트의 Position이 움직인다 가정했을 때,
 어떤 클래스가 조종하는지 파악하기 힘든 경우가 생길 수 있습니다.

  - 예시 - UI에서 채팅창 클래스가 채팅입력을 위해 UI.Input 컴포넌트를 참조하고 있을 때

```      
올바른 예시
- Panel_채팅창클래스
  ㄴ Image_채팅배경
    ㄴ Input_채팅입력 ( 채팅창 클래스가 부모이다. )
    ㄴ Text_채팅메세지

잘못된 예시
- Panel_채팅창클래스
  ㄴ Image_채팅배경

- Input_채팅입력 ( 채팅창 클래스와 부모가 같다. )
- Text_채팅메세지
```

---
## 4. 스크립팅

**4-1. 외부 코드는 되도록이면 수정하지 않습니다.**

  - 외부 에셋 코드를 수정하여, 외부 에셋을 몇년간 업데이트를 못하는 프로젝트도 있습니다.
   이를 방지하기 위함입니다.
  - 원하는 기능은 상속받아서 구현하거나, 래핑해서 구현합니다.
  - 버그 수정의 경우 주석을 반드시 작성합니다.

**4-2. 두개의 문자 이상인 경우 머리 글자로 된 첫 번째 문자만 대문자로 표시합니다.**
```csharp
int iSessionCount;
```

**4-3. 클래스, 변수, 함수의 순서는 템플릿 규약에 있는 포멧을 지킵니다.**

**4-4. Enum의 명명법의 경우 E (Enum)을 붙이며, 변수는 e로 작성합니다.**

**4-5. Enum Flag의 경우 형식 명에 Flag를 포함해야 합니다. 변수 역시 Flag 기능을 사용할 경우 Flag를 접미어로 붙입니다.**

```
[System.Flags]
enum EExportTypeFlags
{
	CSV,
	TXT,
	DB
}

EExportTypeFlags _eExportTypeFlags;
```

**4-6. Delegate의 명명법의 경우 On + 기능으로 작명합니다.**

---
## 5. 스크립팅 - 클래스

**5-1. 클래스 명은 접두어로 프로젝트 명을 붙입니다.**

- Namespace를 대신하는 외부 코드와 나누기 위함입니다.
```
- 프로젝트 명이 RoolingBall의 경우   : RB
- 프로젝트 명이 ShootingGame의 경우  : SG
```

**5-2. 클래스가 abstract 혹은 상속을 통해서 재기능을 발휘해야 할 경우 접미어로 Base를 붙입니다.**
```
- CManagerFrameWorkBase
- CSingletonMonoBase
```

**5-3. 인터페이스의 명명법의 경우 I (Interface)를 붙입니다.**

**5-4. 인터페이스를 상속받는 함수의 경우 가급적이면 해당 인터페이스 타입명을 접두어로 붙입니다.**

```csharp
interface IClockListener
{
	void IClockListener_OnChangeClock(int iHour, int iMinute);
}
```

---
## 6. 스크립팅 - 변수

**6-1. 변수 명명은 헝가리안을 기반으로 하지만, 유니티 C#에만 있는 경우 조금 변경된 스타일로 작성합니다.**
  - 클래스 혹은 스트럭트는 접두어 p를 사용합니다.
```
- 기본형의 경우 ( 유니티 C# 기준 )
  int : i, string : str, Vector : vec, bool : b

- 지역 변수 혹은 함수 파라미터의 경우
  pSomthingClass, pSomthingStruct

- 멤버 변수의 경우
  _iSomthingInt, _pSomthingStruct

- List, Stack 등의 경우
  List<int> listSomthing
  Stack<int> stackSomthing

- Dictionary 의 경우 예외로 map을 사용합니다. ( Cpp 프로젝트와 병행할 수 있기 때문 )
  Dictionary<int, int> mapSomthing
```


**6-2. 외부로 노출하는 변수의 경우 p_ + 변수 명명법으로 작성합니다.**

```csharp
// public int iMinute; // 되도록이면 피합시다.
public int p_iMinute { get; protected set; } // 좋은 예시
```

**6-3. var는 되도록이면 기피하되, 반복문 혹은 아주 짧은 함수에만 작성합니다.**
```csharp
void SmothingShortFunction()
{
  var pIter = listSomthing.GetEnumerator();
  while(pIter.MoveNext())
  {
    // Work Somthing
  }
}
```

**6-4. 상수는 const_ + 변수 명명법으로 작성합니다.**
```csharp
const int const_iSomthing = 1;
const string const_strSomthing = "";
```

**6-4. 가급적이면 매직넘버를 쓰지 말고 상수를 선언합니다.**
```csharp
	// int iClockMaximum = 24;
	const int const_iClockMaximum;
	int iClockMaximum = const_iClockMaximum;
```

**6-5. event 명명법의 경우 p_Event_ + 기능으로 작명합니다.**

  - `p_`만 작성 후에 Visual Studio의 자동 완성(인텔리센스) 기능을 활용하기 위함입니다.

```csharp
delegate void OnChangeClock(int iHour, int iMinute);
event OnChangeClock p_Event_OnChangeClock;
```

**6-6. int 혹은 float의 경우 제한이 있으면 그 제한을 명시합니다.**
```csharp
int iPercent_1_100;
float fProgress_0_1;
int iCardCount_1_5;
```

---
## 7. 스크립팅 - 함수

**7-1. 함수 이름은 접두어가 있는경우 접두어부터 + 동사 + 명사 순으로 작명합니다.**
```
	ㄱ. public 함수의 경우 접두어 Do
		DoPlayEffect, DoPlaySound, DoGetSomthing 등

	ㄴ. non-public Getter, Setter 함수의 경우
		GetSomthing, SetSomthing

	ㄷ. 후크 혹은 이벤트 함수의 경우 접두어 On
		OnClickButton, OnPressButton

	ㄹ. 그 외 protected & private의 경우
		Process_Somthing
```

**7-2. 함수 전개 중, 메소드 내부에 체크가 필요할 시 체크부터 전개 후 리턴합니다. ( 중괄호를 최대한 줄입니다. )**

```csharp
void Process_Clock(int iHour, int iMinute)
{
	if(iHour > 24 || iMinute > 60)
		return;

	Process_Clock_Hour(iHour);
	Process_Clock_Minute(iMinute);
}
```

**7-3. 함수에서 Null을 리턴하는 경우 접미어로 `_OrNull`을 붙입니다.**
```
GetSomthing_OrNull()
```

**7-4. 주석은 왜 이런식으로 전개했는지만 작성하고(Why), 로직에 대한 설명은 함수명으로 대체할 수 있도록 합니다.** ( 드래그로 영역 지정 후 VS - 빠른 작업 - 메서드 추출로 쉽게 할 수 있습니다. )
  - [작성된 코드(CharacterController2D) 참고 링크](https://github.com/KorStrix/StrixLibrary/blob/master/01.CoreCode/2DPlatforming/CCharacterController2D.cs)

```csharp

// 잘못된 예시
void Update()
{
  // 떨어지는 중인지 체크
  if(p_bIsGround == false && _bIsWallSliding == false)
  {
      // Falling 로직 전개
  }
}

// 올바른 예시
void Update()
{
  if(Check_IsFalling())
  {
    // Falling 로직 전개
  }
}

private bool Check_IsFalling()
{
  return p_bIsGround == false && _bIsWallSliding == false;
}
```

---
## 참고 목록
- 포프님 코딩 스탠다드 (한글 번역): http://lonpeach.com/2017/12/24/CSharp-Coding-Standard/#1-%ED%81%B4%EB%9E%98%EC%8A%A4-%EB%B0%8F-%EA%B5%AC%EC%A1%B0%EC%B2%B4%EC%97%90-%ED%8C%8C%EC%8A%A4%EC%B9%BC-%EC%BC%80%EC%9D%B4%EC%8A%A4-%EC%82%AC%EC%9A%A9
- 포프님 코딩 스탠다드 (원문)	 : https://docs.google.com/document/d/1ymFFTVpR4lFEkUgYNJPRJda_eLKXMB6Ok4gpNWOo4rc/edit

---
#### 라이브러리로 돌아가기
https://github.com/KorStrix/StrixLibrary
