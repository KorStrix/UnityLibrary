## 루트 클래스

![](https://postfiles.pstatic.net/MjAxODA1MDdfMjEx/MDAxNTI1NjY0ODU2NTc2.pl7EYZNbYzETHR2t9lPE6O2b6gp9eBCOSWRDpAQmvrEg.km7IoWpo9ucfK6lREhRdZ_gEbi_QIkF0QLRt_lxJZ_0g.JPEG.strix13/StrixLibrary_-_%EB%A3%A8%ED%8A%B8_%ED%81%B4%EB%9E%98%EC%8A%A4.jpg?type=w773)

#### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.



[![Video Label](http://img.youtube.com/vi/xuhKn5H6ck4/0.jpg)](https://www.youtube.com/watch?v=xuhKn5H6ck4=0s)

#### ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다.



- 기존 Unity의 **MonoBehaviour의 기능을 확장** 시킨 루트 클래스입니다.
- **주요 기능**
  - AwakeCoroutine, EnableCoroutine 기능
  - 외부에서 Awake 호출 ( 이미 Awake를 실행한 경우 한번 더 실행유무도 지원 )
  - GetComponentAttribute 지원 ( Awake, Inspector 등에서 할당하지 않고 Attribute로 한줄 작성 )
  - 자체적인 Update를 통해 Update를 사용하는 Object의 실시간 개수 파악과 퍼포먼스 개선


- 이 클래스를 상속받으면 **GetComponentAttribute를 지원**합니다.
  - GetComponentAttribute는 GetComponent, GetComponentInParents, GetComponentInChildren이 있습니다.
  - GetComponentInChildren은 자식 중 첫번째를 찾기, 이름으로 찾기, **복수형 자료형**을 지원하며, **Dictionary도 지원**합니다.
  - Dictionary의 경우 **string을 키값으로 두면 오브젝트의 이름, Enum으로 두면 오브젝트의 이름을 Enum으로 파싱에 성공한 것들만** 저장합니다.
  **- 만약 하나도 못찾을 시 경고 로그를 출력합니다.**

-
- 그 외 UnityEvent 함수를 가상함수로 지원합니다.

---
### 예제
- [작성한 Example 코드](https://github.com/KorStrix/UnityLibrary/tree/master/01.CoreCode/ObjectBase/Example)

---
### 참고 링크

- [참고 링크 - 유니티 블로그 - 10000번의 Update() 호출](https://blogs.unity3d.com/kr/2015/12/23/1k-update-calls/)
- [참고 링크 - Unity3D 자동 GetComponent](https://openlevel.postype.com/post/683269)
---

#### 라이브러리로 돌아가기
https://github.com/KorStrix/UnityLibrary
