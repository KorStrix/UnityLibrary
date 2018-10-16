# 환영합니다
- 이 프로젝트는 Unity C# 오픈소스 라이브러리 프로젝트입니다.

- 이 프로젝트는 게임을 보다 빠르고 편하게 만들기 위한 클래스와 함수를 제공합니다.

- 이 프로젝트는 유니티 내 어떤 프로젝트에도 종속되지 않습니다.

- 이 프로젝트는 프로그래머를 대상으로 설명합니다.

- **이 프로젝트의 유니티 워크 스탠다드 링크**
https://github.com/strix13/UnityLibrary/tree/master/99.Docs/UnityWorkStandard

- 이 문서의 깃허브 링크 :
https://github.com/strix13/UnityLibrary

---
# 목차
- [루트 클래스](https://github.com/strix13/UnityLibrary#%EB%A3%A8%ED%8A%B8-%ED%81%B4%EB%9E%98%EC%8A%A4)
- [UI Panel 시스템 (NGUI 래핑, UGUI 래핑)](https://github.com/strix13/UnityLibrary#ui-panel)
- [로컬라이징 시스템](https://github.com/strix13/UnityLibrary#%EB%A1%9C%EC%BB%AC-%EB%9D%BC%EC%9D%B4%EC%A7%95-%EC%8B%9C%EC%8A%A4%ED%85%9C)
- [사운드 시스템](https://github.com/strix13/UnityLibrary#%EC%82%AC%EC%9A%B4%EB%93%9C-%EC%8B%9C%EC%8A%A4%ED%85%9C)
- [이펙트 시스템](https://github.com/strix13/UnityLibrary#%EC%9D%B4%ED%8E%99%ED%8A%B8-%EC%8B%9C%EC%8A%A4%ED%85%9C)
- [유니티 마크다운 뷰어](https://github.com/strix13/UnityLibrary#%EC%9C%A0%EB%8B%88%ED%8B%B0-%EB%A7%88%ED%81%AC%EB%8B%A4%EC%9A%B4-%EB%B7%B0%EC%96%B4)
- [유니티 로그 래핑 클래스](https://github.com/strix13/UnityLibrary#%EC%9C%A0%EB%8B%88%ED%8B%B0-%EB%A1%9C%EA%B7%B8-%EB%9E%98%ED%95%91-%ED%81%B4%EB%9E%98%EC%8A%A4)
- [랜덤 관련 클래스](https://github.com/strix13/UnityLibrary#%EB%9E%9C%EB%8D%A4-%EA%B4%80%EB%A0%A8-%ED%81%B4%EB%9E%98%EC%8A%A4)
- [한글 파서](https://github.com/strix13/UnityLibrary#%ED%95%9C%EA%B8%80-%ED%8C%8C%EC%84%9C)
- [Tween](https://github.com/strix13/UnityLibrary#tween)
- [그외](https://github.com/strix13/UnityLibrary#%EA%B7%B8-%EC%99%B8)
---
## [루트 클래스](https://github.com/strix13/UnityLibrary#%EB%AA%A9%EC%B0%A8)

[![Video Label](http://img.youtube.com/vi/xuhKn5H6ck4/0.jpg)](https://www.youtube.com/watch?v=xuhKn5H6ck4=0s)

### ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다.



- 기존 Unity의 **MonoBehaviour의 기능을 확장** 시킨 루트 클래스입니다. ( MonoBehaviour 대용 )
- **루트 클래스의 주요 기능**
  - AwakeCoroutine, EnableCoroutine 기능
  - 외부에서 Awake 호출 ( 이미 Awake를 실행한 경우 한번 더 실행유무도 지원 )
  - GetComponentAttribute 지원 ( Awake, Inspector 등에서 할당하지 않고 Attribute로 한줄 작성 )
  - 자체적인 Update를 통해 Update를 사용하는 Object의 실시간 개수 파악과 퍼포먼스 개선

- [작성한 코드 및 좀 더 알아보기 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCode/ObjectBase)

---
## [UI Panel](https://github.com/strix13/UnityLibrary#%EB%AA%A9%EC%B0%A8)

![](
https://blogfiles.pstatic.net/MjAxODA1MDVfNSAg/MDAxNTI1NDk2MjEzMzk4.YBGR9hSvFoGqEg5lUCeF346bvZ3x9EEgLQSfjWcyFPsg.DdcKOk5Ml-eeOorUOfqwJcJnscZGxqmvC_Ol40H9eZ4g.PNG.strix13/StrixLibrary_-_UI_%ED%81%B4%EB%9E%98%EC%8A%A4_%EA%B4%80%EA%B3%84%EB%8F%84.png)

### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.




- 구조는 크게 Manager, Panel로 이루어져 있으며 Manager는 Panel을 관리하는 싱글톤 패턴입니다.
- NGUI 버젼, UGUI 버젼을 구분하였으며 모두 같은 클래스와 인터페이스를 상속받게 하였습니다.
- Panel의 경우 NGUI로는 NGUIPanel, UGUI로는 Canvas 단위로 된 하나의 Window 단위 입니다.

- **Manager 주요 기능**
  - Singleton - 언제 어디서든 호출 가능
  - Panel의 Show, Hide 함수 지원
      - Sorting Layer 관리
      - Show, Hide Animation Coroutine 지원
  - Panel FadeIn / Out 효과 지원
  - 해당 Panel Class Get 지원

- **Panel 주요 기능**
  - 하위 **UI Element의 이벤트 ( 버튼, DropItem, Input 입력 ) 등을 쉽게 override하여 쉽게 사용할 수 있도록 지원**
  - **UI Element를 쉽게 Get** 할 수 있도록 지원
  - OnShow / OnHide의 이벤트가 있으며, **Show / Hide 시 코루틴이 동작합니다. ( 애니메이션을 기다리기 위함, 코루틴 끝날 때 콜백 지원 )**

- [작성한 UI 베이스 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/02.UI)
- [작성한 UGUI 베이스 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/03.UGUI)
- [작성한 NGUI 베이스 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/10.ExternalLibraryWrapper/NGUI)

---
## [로컬 라이징 시스템](https://github.com/strix13/UnityLibrary#%EB%AA%A9%EC%B0%A8)

[![Video Label](http://img.youtube.com/vi/mLQMwqKgh4I/0.jpg)](https://www.youtube.com/watch?v=mLQMwqKgh4I=0s)

### ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다.




- 로컬라이징의 경우 Key, Value를 가진 임의의 Text파일을 파싱한 데이터를 기반으로 동작합니다.

- 외부에서 Instance 요청 시 자동으로 생성되서 파일을 파싱 후, 자동으로 로컬라이징이 될 수 있게끔 작업하였습니다.

- 작성한 Text, Sprite 컴포넌트 외에 이벤트를 받고 싶으면 ILocalizeListner를 구현하고, Manager에 등록하면 됩니다.

- [작성한 코드 및 더 알아보기 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCode/Localize)

---
## [사운드 시스템](https://github.com/strix13/UnityLibrary#%EB%AA%A9%EC%B0%A8)

[![Video Label](http://img.youtube.com/vi/TN145PFwvkI/0.jpg)](https://www.youtube.com/watch?v=TN145PFwvkI=0s)

### ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다.

- 사운드 시스템의 경우, SoundManager가 SoundSlot 클래스를 관리하며, 스크립트 혹은 Sound Player를 통해 사운드를 재생합니다.
- [작성한 코드 및 더 알아보기 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCode/Sound)

---
## [이펙트 시스템](https://github.com/strix13/UnityLibrary#%EB%AA%A9%EC%B0%A8)

![](
https://postfiles.pstatic.net/MjAxODA1MDZfMzUg/MDAxNTI1NjEwMDU3MjQz.az-Y7SxCGDV-jlb4YCi06dZMTi5DQpIbyeBIzaLJwJkg.kfdc4L_DxkRORmGBFbGdlu6vHzu1MqnzrZKj3CL_kZIg.JPEG.strix13/StrixLibrary_-_%EC%9D%B4%ED%8E%99%ED%8A%B8_%EC%8B%9C%EC%8A%A4%ED%85%9C_%ED%81%B4%EB%9E%98%EC%8A%A4_%EA%B4%80%EA%B3%84%EB%8F%84.jpg?type=w773)

### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.




[![Video Label](http://img.youtube.com/vi/OQ8UpqBUIJQ/0.jpg)](https://www.youtube.com/watch?v=OQ8UpqBUIJQ=0s)

### ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다.

- 이펙트 시스템은 사운드 시스템과 비슷한 설계입니다. 매니져와, 리소스 래퍼와, 플레이어로 이루어져 있습니다.

- [작성한 코드 및 좀 더 알아보기 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCode/Effect)

---
## [유니티 마크다운 뷰어](https://github.com/strix13/UnityLibrary#%EB%AA%A9%EC%B0%A8)

![](https://camo.githubusercontent.com/6651c1a8b118bb47b030aa998be60db73d057f8d/68747470733a2f2f706f737466696c65732e707374617469632e6e65742f4d6a41784f4441314d4456664d6a4d342f4d4441784e5449314e5445354e4459354e4441312e775252445f66685450675239423470716b71496151574b4e4d586b543435757a575f526753323272494c30672e45756153622d417733516552434b4e73795f4d6643357566475561456f6448674d666435335550766f724d672e4a5045472e737472697831332f62616e646963616d5f323031382d30352d30355f32302d31392d32302d3732392e6a70673f747970653d77373733)

#### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.


- 유니티 내에서 마크다운 문서를 읽기 위해 만들었습니다.
- [작성한 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCode/Editor/MarkdownViewer)

---
## [유니티 로그 래핑 클래스](https://github.com/strix13/UnityLibrary#%EB%AA%A9%EC%B0%A8)

![](https://postfiles.pstatic.net/MjAxODA1MDdfMjcg/MDAxNTI1Njc5NTI2ODE1.x-IjrZGPCJ9jE_2Vbq-Z8V7RM47fA8ixYiP2mGsnbHQg.b41kzPpZpwXmpUiz0q69tnt3LehiBJ2zik8RpPxTCQQg.JPEG.strix13/bandicam_2018-05-07_16-52-00-886.jpg?type=w773)
![](
https://postfiles.pstatic.net/MjAxODA1MDdfMTc4/MDAxNTI1Njc5NDU5MzMz.4WXQCeIXmSfGTzxJ7HsOJr8adwepP0JlfNOIrT7vR70g.iXFhfhPCRYxKJVHB7oOmKfpU14Aei4eNv-GbfphEZp0g.PNG.strix13/%EB%A1%9C%EA%B7%B8.png?type=w773)

### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.




- 로그 작성자, 로그 레벨에 따라 로그를 출력하지 않는 필터링 기능을 지원합니다.

- **로그 발생 시 txt, csv, DB로 동시 출력할 수 있는 기능** 을 지원합니다.
  - 파일에 로그 발생 시간과 콜스택도 기록합니다.

- 차후 Conditional을 도입할 예정입니다.

- [작성한 코드 링크](https://github.com/KorStrix/Unity_DebugWrapper)

---
## [랜덤 관련 클래스](https://github.com/strix13/UnityLibrary#%EB%AA%A9%EC%B0%A8)
### 랜덤 매니져

- 랜덤 매니져는 IRandomItem 을 구현한 클래스에 대해 랜덤으로 리턴해주는 관리자입니다.

- **IRandomItem의 경우 랜덤확률을 구현하게끔 되어있으며, 랜덤확률에 따라 클래스를 리턴** 합니다.

- **랜덤모드로 Peek, Delete 모드를 지원** 합니다.
  - Peek의 경우 Random Item List에서 **아이템을 뽑을 때, List에 지장이 없습니다.**
  - Delete의 경우 List에서 **아이템을 뽑을 때 해당 아이템은List에서 제거됩니다.**

### 랜덤 2D 섹터

- 섹터의 경우, 인스펙터에서 몇 바이 몇 섹터인지, 섹터 당 사이즈가 몇인지를 설정합니다.

- 그 외 주변 섹터 체크옵션으로 주위 4타일(위 아래, 양 옆), 주위 8타일(4타일에서 대각선 4타일 추가)를 지원합니다.

- 섹터에게 랜덤 포지션을 요청 시, **직전의 섹터가 같은 섹터가 아니고, 주변 섹터도 아닌 섹터 내의 랜덤 위치만 리턴**합니다.

- **이펙트, 몬스터 스폰 등에 쓰입니다.** ( 스폰 시 주변에 골고루 나오게 하기 위한 로직 )

- [작성한 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCode/Random)

---
## [한글 파서](https://github.com/strix13/UnityLibrary#%EB%AA%A9%EC%B0%A8)

![](
https://postfiles.pstatic.net/MjAxODA1MDdfNjAg/MDAxNTI1NjgzNDYwNTkx.7LVx1ndhvjxdw2992EqJKUM_u6ZZSuYqaGKxwlNv-Cwg.ieXZO9IUNCEUPh_ut5VM-TEf1GaUJknekDBuKTyQUxIg.JPEG.strix13/bandicam_2018-05-07_17-57-24-601.jpg?type=w773)

#### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.



- VR, 모바일 등에서 커스터마이징 된 키보드로 한글 혹은 영어를 입력할 수 있기 위해 제작하였습니다.

- 한글Char와 한글String으로 이루어져 있습니다.

- 한글Char의 경우 한글의 초, 중, 종성 및 모음 등을 파싱합니다.

- [참고한 코드 링크 1](http://plog2012.blogspot.kr/2012/11/c.html)

- [참고한 코드 링크 2](http://ehclub.co.kr/2484?category=658554)

- [작성한 코드 링크](https://github.com/strix13/UnityLibrary/blob/master/01.CoreCode/HangleParser/SHangul.cs)

---
## [Tween](https://github.com/strix13/UnityLibrary#%EB%AA%A9%EC%B0%A8)

![](https://postfiles.pstatic.net/MjAxODA4MzBfMjA0/MDAxNTM1NTkzMTU0NDY5.MViQaLuMEAPWTndcryjA5nPCrqt41XAdx7etDxTzdscg.G8W_Jbnc3CD-OEqLzu34NpF69ORxHBp_vonWVFHMr30g.GIF.strix13/TweenPlay.gif?type=w773)

![](
https://postfiles.pstatic.net/MjAxODA4MzBfMjk5/MDAxNTM1NTkyOTQ5NDA1.n09HmR-JsfqCLMeSnddWhTWJlPje_eUlA_PAjughF08g.F9VbH9azaoKUpUev2L-CLqtUbd6r_OuCULxlGgy9sY0g.GIF.strix13/TweenButton.gif?type=w773)



NGUI의 트윈을 참고하여 제작한 Tweener입니다.
NGUI 트위너에 없는 기능으로
  - Editor Play ( 단일 트윈 재생, 현재 오브젝트에 있는 모든 트윈 동시 재생 )
  - 트윈 스타일 - PingPong_ReverseCurve 기능
    - 애니메이션 커브를 Forward와 Reverse 둘다 같은 커브 적용
    - 기존 핑퐁은 Reverse의 경우 커브를 역행하며 트윈하지만, 이 스타일은 Reverse도 순행하며 트윈
  - FixedUpdate를 사용할 것인지
  - Rotation의 경우 Vector Lerp를 사용할 것인지 Quaternion Lerp을 사용할 것인지
  - Radiaul Direction Objects Tween
  - 어느 트윈을 어떻게 사용하는지 일부 Gizmo 지원

  - 그 외 쉬운 확장 ( CTweenBase를 상속받아 몇개의 함수만 구현하면 끝 [Tween Scale 참고](https://github.com/strix13/UnityLibrary/blob/master/01.CoreCode/Tween/CTweenScale.cs) )

---
**개선 해야 할 사항**
- 트윈 데이터를 ScriptableObject와 Inspector에서 즉석으로 세팅할 수 있도록 병행  (Bolt 참고)
- 한 컴포넌트에 여러 트윈 데이터를 저장 및 실행

- [작성한 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCode/Tween)

---
## [그 외](https://github.com/strix13/UnityLibrary#%EB%AA%A9%EC%B0%A8)

![](https://postfiles.pstatic.net/MjAxODA1MDdfMjM4/MDAxNTI1NjgyMzE3Nzgw.yGbOqrWKQ8zXPQuGo4GGgAF3CFvL0wNJdtdMWfKFQEcg.1zO8gO6AaNemOFtekPgnoqSigNlRyWJ0jOE4tuYiqaYg.JPEG.strix13/bandicam_2018-05-07_17-38-28-987.jpg?type=w773)

- 최근에 작성한 코드부터는 Test 코드를 작성하기 시작했습니다.

---
읽어주셔서 감사합니다.


### 이 프로젝트 내 다른 링크
- **이 프로젝트의 유니티 워크 스탠다드 링크**
https://github.com/strix13/UnityLibrary/tree/master/99.Docs/UnityWorkStandard


### 개발자 연락 및 활동처

- 카카오톡 ( 유니티 개발자 모임 ) : https://open.kakao.com/o/gOi17az

- 디스코드 ( 유니티 개발자 모임 ) : https://discord.gg/9EGrJap
