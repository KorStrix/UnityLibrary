
## [사운드 시스템](https://github.com/KorStrix/UnityLibrary#%EB%AA%A9%EC%B0%A8)

![](
https://postfiles.pstatic.net/MjAxODA1MDZfODIg/MDAxNTI1NTg2NjI4NDg5.2-piThykc2EWXJVPdEQUx0FlQ9PoSANx5ZLP1S8-KWwg.vvcEPNS5G5_jlnjJqcXHSgF2I94o_bMPaWWPa4537BEg.JPEG.strix13/StrixLibrary_-_%EC%82%AC%EC%9A%B4%EB%93%9C_%EC%8B%9C%EC%8A%A4%ED%85%9C_%ED%81%B4%EB%9E%98%EC%8A%A4_%EA%B4%80%EA%B3%84%EB%8F%84.jpg?type=w773)

### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.




[![Video Label](http://img.youtube.com/vi/TN145PFwvkI/0.jpg)](https://www.youtube.com/watch?v=TN145PFwvkI=0s)

### ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다.




- 사운드 시스템의 경우, SoundManager가 SoundSlot 클래스를 관리하며, 스크립트 혹은 Sound Player를 통해 사운드를 재생합니다.
- **사운드 매니져 주요 기능**
  - Singleton - 언제 어디서든지 호출 가능
  - SoundSlot을 통해 AudioClip 재생, SoundSlot은 풀링
  - 현재 설정된 BGM Volume, Game Effect Volume, 몇개의 SoundSlot이 플레이 중인지 지원 [ Editor Only ]
  - Sound의 Mute, FadeIn, Out 지원
  - 그룹을 설정하여 랜덤 Sound 플레이 기능

---
- **사운드 슬롯 주요 기능**
  - AudioSource 래핑한 클래스
  - Local Volume, AudioClip이 끝날 때 Event 통지
  - 현재 재생중인지, 총 몇초 중 몇초 플레이 중인지 지원 [ Editor Only ]
---
- **사운드 플레이어 주요 기능**
  - 인스펙터에 세팅하여 유니티 주요 이벤트 ( Awake, Enable, Disable 등 )시에 사운드 재생
  - Key(string) - AudioClip 로 세팅하여 한 플레이어에 임의의 AudioClip 실행 가능
  - 어떤 사운드 슬롯을 통해 실행되는지 확인 가능 [ Editor Only ]
  - 현재 재생중인지 지원 [ Editor Only ]

---

#### 라이브러리로 돌아가기
https://github.com/KorStrix/UnityLibrary
